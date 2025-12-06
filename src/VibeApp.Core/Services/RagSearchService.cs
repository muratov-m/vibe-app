using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using VibeApp.Core.DTOs;
using VibeApp.Core.Entities;
using VibeApp.Core.Interfaces;

namespace VibeApp.Core.Services;

/// <summary>
/// Service for RAG (Retrieval Augmented Generation) search across user profiles
/// </summary>
public class RagSearchService : IRagSearchService
{
    private readonly IRepository<UserProfileEmbedding> _embeddingRepository;
    private readonly IRepository<UserProfile> _userProfileRepository;
    private readonly IOpenAIGateway _openAIGateway;
    private readonly ILogger<RagSearchService> _logger;

    public RagSearchService(
        IRepository<UserProfileEmbedding> embeddingRepository,
        IRepository<UserProfile> userProfileRepository,
        IOpenAIGateway openAIGateway,
        ILogger<RagSearchService> logger)
    {
        _embeddingRepository = embeddingRepository;
        _userProfileRepository = userProfileRepository;
        _openAIGateway = openAIGateway;
        _logger = logger;
    }

    public async Task<RagSearchResponseDto> SearchAsync(RagSearchRequestDto request, CancellationToken cancellationToken = default)
    {
        // Validate input parameters
        if (string.IsNullOrWhiteSpace(request.Query))
            throw new ArgumentException("Query cannot be empty", nameof(request.Query));

        if (request.TopK <= 0)
            throw new ArgumentException("TopK must be positive", nameof(request.TopK));

        if (request.MinSimilarity < 0 || request.MinSimilarity > 1)
            throw new ArgumentException("MinSimilarity must be between 0 and 1", nameof(request.MinSimilarity));

        _logger.LogInformation("RAG search started for query: {Query}", request.Query);

        // Step 1: Generate embedding for the query
        var queryEmbedding = await GenerateQueryEmbeddingAsync(request.Query, cancellationToken);
        var queryVector = new Vector(queryEmbedding);

        // Step 2: Build base query for embeddings
        var embeddingsQuery = _embeddingRepository.GetQueryable();

        // Step 3: Apply structured filters if provided (using JOIN for better performance)
        if (request.Filters != null)
        {
            var profileQuery = _userProfileRepository.GetQueryable();

            // Apply Country filter
            if (!string.IsNullOrWhiteSpace(request.Filters.Country))
            {
                profileQuery = profileQuery.Where(p => p.Country == request.Filters.Country);
            }

            // Apply HasStartup filter
            if (request.Filters.HasStartup.HasValue)
            {
                profileQuery = profileQuery.Where(p => p.HasStartup == request.Filters.HasStartup.Value);
            }

            // JOIN embeddings with filtered profiles
            embeddingsQuery = embeddingsQuery
                .Join(
                    profileQuery,
                    e => e.UserProfileId,
                    p => p.Id,
                    (e, p) => e
                );
        }

        // Step 4: Search for similar profile IDs using cosine distance
        // Apply similarity filter, sort by distance, and take TopK
        var similarEmbeddings = await embeddingsQuery
            .Select(e => new
            {
                e.UserProfileId,
                Distance = e.Embedding.CosineDistance(queryVector)
            })
            .Where(e => (1 - e.Distance) >= request.MinSimilarity)
            .OrderBy(e => e.Distance)
            .Take(request.TopK)
            .ToListAsync(cancellationToken);

        if (!similarEmbeddings.Any())
        {
            _logger.LogInformation("No profiles found matching query with similarity >= {MinSimilarity}", request.MinSimilarity);
            return new RagSearchResponseDto
            {
                Query = request.Query,
                Results = new List<ProfileSearchResultDto>(),
                TotalResults = 0,
                Answer = request.GenerateResponse 
                    ? "I couldn't find any profiles matching your query. Try broadening your search or using different keywords."
                    : null
            };
        }

        // Step 5: Load full profile data for matched IDs (separate optimized query)
        var matchedProfileIds = similarEmbeddings.Select(e => e.UserProfileId).ToList();
        var distanceMap = similarEmbeddings.ToDictionary(e => e.UserProfileId, e => e.Distance);

        var profiles = await _userProfileRepository.GetQueryable()
            .Include(p => p.Skills)
            .Include(p => p.LookingFor)
            .Where(p => matchedProfileIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {Count} profiles matching query with similarity >= {MinSimilarity}",
            similarEmbeddings.Count, request.MinSimilarity);

        // Step 6: Map to response DTOs (preserving similarity score order)
        // Use Dictionary for O(1) lookup instead of .First() in loop
        var profileMap = profiles.ToDictionary(p => p.Id);
        
        var results = matchedProfileIds
            .Where(id => profileMap.ContainsKey(id))
            .Select(id => profileMap[id])
            .Select(p => new ProfileSearchResultDto
            {
                Id = p.Id,
                Name = p.Name,
                Bio = p.Bio,
                Skills = p.Skills?.Select(s => s.Skill).ToList() ?? new List<string>(),
                LookingFor = p.LookingFor?.Select(l => l.LookingFor).ToList() ?? new List<string>(),
                Telegram = p.Telegram,
                LinkedIn = p.LinkedIn,
                Email = p.Email,
                City = p.City,
                Country = p.Country,
                HasStartup = p.HasStartup,
                StartupName = p.HasStartup ? p.StartupName : null,
                StartupStage = p.HasStartup ? p.StartupStage : null,
                CanHelp = p.CanHelp,
                NeedsHelp = p.NeedsHelp,
                SimilarityScore = (float)(1 - distanceMap[p.Id])
            }).ToList();

        var response = new RagSearchResponseDto
        {
            Query = request.Query,
            Results = results,
            TotalResults = results.Count
        };

        // Step 7: Generate natural language response if requested
        if (request.GenerateResponse)
        {
            response.Answer = await GenerateLlmResponseAsync(request.Query, results, cancellationToken);
        }

        return response;
    }

    public async Task<float[]> GenerateQueryEmbeddingAsync(string query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating embedding for query: {Query}", query);
        return await _openAIGateway.GetEmbeddingAsync(query, cancellationToken: cancellationToken);
    }

    private async Task<string> GenerateLlmResponseAsync(
        string query,
        List<ProfileSearchResultDto> profiles,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating LLM response for {Count} profiles", profiles.Count);

        // Limit profiles to top 10 to avoid exceeding token limits
        var topProfiles = profiles.Take(10).ToList();

        // Build context from matching profiles
        var contextBuilder = new StringBuilder();
        contextBuilder.AppendLine("Here are the relevant user profiles:");
        contextBuilder.AppendLine();

        foreach (var profile in topProfiles)
        {
            contextBuilder.AppendLine($"--- Profile: {profile.Name} (Similarity: {profile.SimilarityScore:P0}) ---");
            
            if (!string.IsNullOrEmpty(profile.Bio))
                contextBuilder.AppendLine($"Bio: {profile.Bio}");
            
            if (profile.Skills.Any())
                contextBuilder.AppendLine($"Skills: {string.Join(", ", profile.Skills)}");
            
            if (profile.LookingFor.Any())
                contextBuilder.AppendLine($"Looking for: {string.Join(", ", profile.LookingFor)}");
            
            if (profile.HasStartup && !string.IsNullOrEmpty(profile.StartupName))
                contextBuilder.AppendLine($"Startup: {profile.StartupName} ({profile.StartupStage})");
            
            if (!string.IsNullOrEmpty(profile.CanHelp))
                contextBuilder.AppendLine($"Can help with: {profile.CanHelp}");
            
            if (!string.IsNullOrEmpty(profile.NeedsHelp))
                contextBuilder.AppendLine($"Needs help with: {profile.NeedsHelp}");
            
            if (!string.IsNullOrEmpty(profile.Telegram))
                contextBuilder.AppendLine($"Contact: @{profile.Telegram.TrimStart('@')}");
            
            contextBuilder.AppendLine();
        }

        var systemPrompt = @"You are a helpful assistant for a networking community app. 
Your job is to answer questions about community members based on their profile information.
Be concise, friendly, and helpful. When mentioning people, include their Telegram handle if available.
If the query asks for specific skills or interests, highlight the most relevant matches first.
Always respond in the same language as the user's query.";

        var userPrompt = $@"User's question: {query}

{contextBuilder}

Based on the profiles above, answer the user's question. Mention the most relevant people and briefly explain why they match.";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };

        try
        {
            // Create a timeout for LLM call (30 seconds)
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(30));

            var response = await _openAIGateway.CreateChatCompletionAsync(
                messages,
                model: "gpt-4.1-nano",
                temperature: 0.2f,
                cancellationToken: timeoutCts.Token);

            return response;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("LLM response generation timed out after 30 seconds");
            return $"Found {profiles.Count} matching profiles. Check the results list for details. (Response generation timed out)";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating LLM response");
            return $"Found {profiles.Count} matching profiles. Check the results list for details.";
        }
    }
}

