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

            // Apply Country filter (using parsed country from AI)
            if (!string.IsNullOrWhiteSpace(request.Filters.Country))
            {
                profileQuery = profileQuery.Where(p => p.ParsedCountry == request.Filters.Country);
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
        // Sort by distance and take TopK (no minimum similarity threshold)
        var similarEmbeddings = await embeddingsQuery
            .Select(e => new
            {
                e.UserProfileId,
                Distance = e.Embedding.CosineDistance(queryVector)
            })
            .OrderBy(e => e.Distance)
            .Take(request.TopK)
            .ToListAsync(cancellationToken);

        if (!similarEmbeddings.Any())
        {
            _logger.LogInformation("No profiles found matching query");
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

        _logger.LogInformation("Found {Count} profiles matching query",
            similarEmbeddings.Count);

        // Step 6: Map to response DTOs (preserving similarity score order)
        // Use Dictionary for O(1) lookup instead of .First() in loop
        var profileMap = profiles.ToDictionary(p => p.Id);
        
        var results = matchedProfileIds
            .Where(id => profileMap.ContainsKey(id))
            .Select(id => profileMap[id])
            .Select(p =>
            {
                var distance = distanceMap[p.Id];
                var similarityScore = (float)(1 - distance);
                
                // Ensure valid similarity score (prevent NaN/Infinity)
                if (float.IsNaN(similarityScore) || float.IsInfinity(similarityScore))
                {
                    similarityScore = 0f;
                }
                
                // Clamp to valid range [0, 1]
                similarityScore = Math.Clamp(similarityScore, 0f, 1f);
                
                return new ProfileSearchResultDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Bio = p.Bio,
                    Skills = p.Skills?.Select(s => s.Skill).ToList() ?? new List<string>(),
                    LookingFor = p.LookingFor?.Select(l => l.LookingFor).ToList() ?? new List<string>(),
                    Telegram = p.Telegram,
                    LinkedIn = p.LinkedIn,
                    Email = p.Email,
                    City = p.ParsedCity,
                    Country = p.ParsedCountry,
                    HasStartup = p.HasStartup,
                    StartupName = p.HasStartup ? p.StartupName : null,
                    StartupStage = p.HasStartup ? p.StartupStage : null,
                    CanHelp = p.CanHelp,
                    NeedsHelp = p.NeedsHelp,
                    Interests = p.ParsedInterests,
                    SimilarityScore = similarityScore
                };
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

        // Limit profiles to top 5 to avoid exceeding token limits
        var topProfiles = profiles
            .OrderByDescending(x => x.SimilarityScore)
            .Take(5)
            .ToList();

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

IMPORTANT: Format your response as a structured list of people with brief descriptions.

Format:
- For each person, include: Name (with Telegram if available) → brief description of why they match → key skills/interests
- Use bullet points or numbered list for clarity
- Be concise but informative (2-3 sentences per person)
- Always respond in the same language as the user's query
- Focus on the most relevant matches first

Example format:
1. **John Doe (@johndoe)** — AI/ML эксперт с опытом в стартапах. Специализируется на deep learning и computer vision. Ищет со-основателей.
2. **Jane Smith (@janesmith)** — Backend разработчик с 5+ лет опыта. Знает Rust, Go, Kubernetes. Может помочь с архитектурой систем.";

        var userPrompt = $@"User's question: {query}

{contextBuilder}

Based on the profiles above, provide a structured list of the most relevant people with brief descriptions of why they match the query.";

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

