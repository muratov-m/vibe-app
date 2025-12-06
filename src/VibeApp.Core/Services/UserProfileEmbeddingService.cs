using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using Pgvector;
using VibeApp.Core.DTOs;
using VibeApp.Core.Entities;
using VibeApp.Core.Interfaces;

namespace VibeApp.Core.Services;

public class UserProfileEmbeddingService : IUserProfileEmbeddingService
{
    private readonly IRepository<UserProfile> _userProfileRepository;
    private readonly IRepository<UserProfileEmbedding> _embeddingRepository;
    private readonly IOpenAIGateway _openAIGateway;
    private readonly ILogger<UserProfileEmbeddingService> _logger;

    public UserProfileEmbeddingService(
        IRepository<UserProfile> userProfileRepository,
        IRepository<UserProfileEmbedding> embeddingRepository,
        IOpenAIGateway openAIGateway,
        ILogger<UserProfileEmbeddingService> logger)
    {
        _userProfileRepository = userProfileRepository;
        _embeddingRepository = embeddingRepository;
        _openAIGateway = openAIGateway;
        _logger = logger;
    }

    public async Task GenerateAndSaveEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating embedding for user profile {UserProfileId}", userProfileId);

            // Get user profile with related data (Skills and LookingFor)
            var profile = await _userProfileRepository.GetQueryable()
                .Include(p => p.Skills)
                .Include(p => p.LookingFor)
                .FirstOrDefaultAsync(p => p.Id == userProfileId, cancellationToken);
            
            if (profile == null)
            {
                _logger.LogWarning("User profile {UserProfileId} not found", userProfileId);
                return;
            }

            // Step 1: Parse profile and update structured fields
            try
            {
                await ParseAndUpdateProfileAsync(profile, cancellationToken);
                _logger.LogInformation("Successfully parsed profile {UserProfileId}", userProfileId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing profile {UserProfileId}. Continuing with embedding generation.", userProfileId);
            }

            // Step 2: Generate embedding
            var embedding = await GenerateEmbeddingAsync(profile, cancellationToken);

            // Check if embedding already exists
            var existingEmbedding = await _embeddingRepository.FirstOrDefaultAsync(e => e.UserProfileId == userProfileId);

            if (existingEmbedding != null)
            {
                // Update existing embedding
                existingEmbedding.Embedding = embedding;
                existingEmbedding.UpdatedAt = DateTime.UtcNow;
                await _embeddingRepository.UpdateAsync(existingEmbedding);
                _logger.LogInformation("Updated embedding for user profile {UserProfileId}", userProfileId);
            }
            else
            {
                // Create new embedding
                var newEmbedding = new UserProfileEmbedding
                {
                    UserProfileId = userProfileId,
                    Embedding = embedding,
                    CreatedAt = DateTime.UtcNow
                };
                await _embeddingRepository.AddAsync(newEmbedding);
                _logger.LogInformation("Created new embedding for user profile {UserProfileId}", userProfileId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for user profile {UserProfileId}", userProfileId);
            throw;
        }
    }

    public async Task DeleteEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var embedding = await _embeddingRepository.FirstOrDefaultAsync(e => e.UserProfileId == userProfileId);
            if (embedding != null)
            {
                await _embeddingRepository.DeleteAsync(embedding);
                _logger.LogInformation("Deleted embedding for user profile {UserProfileId}", userProfileId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting embedding for user profile {UserProfileId}", userProfileId);
            throw;
        }
    }

    public async Task<UserProfileEmbedding?> GetEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default)
    {
        return await _embeddingRepository.FirstOrDefaultAsync(e => e.UserProfileId == userProfileId);
    }

    private async Task<Vector> GenerateEmbeddingAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        var profileText = BuildProfileText(profile);
        _logger.LogInformation("Generating embedding for profile: {Name}, text length: {Length}", 
            profile.Name, profileText.Length);

        var embeddingArray = await _openAIGateway.GetEmbeddingAsync(profileText, cancellationToken: cancellationToken);
        
        return new Vector(embeddingArray);
    }

    private static string BuildProfileText(UserProfile profile)
    {
        var sb = new StringBuilder();

        // 1. Bio - MOST IMPORTANT for semantic search
        if (!string.IsNullOrWhiteSpace(profile.Bio))
            sb.AppendLine($"Bio: {profile.Bio}");

        // 2. Looking For - networking goals (high priority)
        if (profile.LookingFor?.Any() == true)
        {
            var lookingFor = string.Join(", ", profile.LookingFor.Select(l => l.LookingFor));
            sb.AppendLine($"Looking for: {lookingFor}");
        }

        // 3. Skills - professional skills
        if (profile.Skills?.Any() == true)
        {
            var skills = string.Join(", ", profile.Skills.Select(s => s.Skill));
            sb.AppendLine($"Skills: {skills}");
        }

        // 4. Startup info
        if (profile.HasStartup)
        {
            if (!string.IsNullOrWhiteSpace(profile.StartupName))
                sb.AppendLine($"Startup: {profile.StartupName}");
            if (!string.IsNullOrWhiteSpace(profile.StartupDescription))
                sb.AppendLine($"About startup: {profile.StartupDescription}");
            if (!string.IsNullOrWhiteSpace(profile.StartupStage))
                sb.AppendLine($"Stage: {profile.StartupStage}");
        }

        // 5. Collaboration
        if (!string.IsNullOrWhiteSpace(profile.CanHelp))
            sb.AppendLine($"Can help: {profile.CanHelp}");
        
        if (!string.IsNullOrWhiteSpace(profile.NeedsHelp))
            sb.AppendLine($"Needs: {profile.NeedsHelp}");

        // 6. AI usage
        if (!string.IsNullOrWhiteSpace(profile.AiUsage))
            sb.AppendLine($"AI: {profile.AiUsage}");

        // 7. Name (at the end - less important for embeddings)
        sb.AppendLine($"Name: {profile.Name}");

        return sb.ToString().Trim();
    }

    private async Task ParseAndUpdateProfileAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        try
        {
            var prompt = BuildParsingPrompt(profile);
            
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(@"You are a profile data extraction assistant. 
Your task is to analyze user profile information and extract structured data.
Always respond with valid JSON only, no additional text.

Response format:
{
  ""shortBio"": ""Brief summary up to 500 characters"",
  ""mainActivity"": ""Main occupation or role"",
  ""interests"": [""interest1"", ""interest2""],
  ""country"": ""Country name"",
  ""city"": ""City name""
}

Rules:
- shortBio: Summarize the most important aspects, keep it under 500 characters
- mainActivity: Extract primary profession or current role
- interests: List key interests, technologies, or focus areas (max 10)
- country: Extract or infer country from bio, location hints, or other context
- city: Extract or infer city from bio, location hints, or other context
- If information is not available, use empty string or empty array
- All text in original language (do not translate)"),
                new UserChatMessage(prompt)
            };

            var response = await _openAIGateway.CreateChatCompletionAsync(
                messages,
                model: "gpt-4.1-nano",
                temperature: 0.2f,
                maxTokens: 500,
                cancellationToken: cancellationToken);

            _logger.LogDebug("OpenAI parsing response: {Response}", response);

            var parsed = JsonSerializer.Deserialize<UserProfileParsedDto>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (parsed != null)
            {
                // Update parsed fields in profile
                profile.ParsedShortBio = parsed.ShortBio;
                profile.ParsedMainActivity = parsed.MainActivity;
                profile.ParsedInterests = string.Join(", ", parsed.Interests);
                profile.ParsedCountry = parsed.Country;
                profile.ParsedCity = parsed.City;
                profile.UpdatedAt = DateTime.UtcNow;
                
                await _userProfileRepository.UpdateAsync(profile);
                
                _logger.LogInformation("Updated parsed fields for profile {UserProfileId}: MainActivity={MainActivity}, Interests={InterestCount}", 
                    profile.Id, parsed.MainActivity, parsed.Interests.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing profile {UserProfileId} with OpenAI", profile.Id);
            // Fallback: use simple extraction
            profile.ParsedShortBio = profile.Bio.Length > 500 ? profile.Bio[..500] : profile.Bio;
            profile.ParsedMainActivity = string.Empty;
            profile.ParsedInterests = profile.Skills?.Any() == true 
                ? string.Join(", ", profile.Skills.Select(s => s.Skill)) 
                : string.Empty;
            profile.ParsedCountry = string.Empty;
            profile.ParsedCity = string.Empty;
            profile.UpdatedAt = DateTime.UtcNow;
            
            await _userProfileRepository.UpdateAsync(profile);
        }
    }

    private static string BuildParsingPrompt(UserProfile profile)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("Parse the following user profile:");
        sb.AppendLine();
        
        if (!string.IsNullOrWhiteSpace(profile.Name))
            sb.AppendLine($"Name: {profile.Name}");
        
        if (!string.IsNullOrWhiteSpace(profile.Bio))
            sb.AppendLine($"Bio: {profile.Bio}");
        
        if (profile.Skills?.Any() == true)
        {
            var skills = string.Join(", ", profile.Skills.Select(s => s.Skill));
            sb.AppendLine($"Skills: {skills}");
        }
        
        if (profile.LookingFor?.Any() == true)
        {
            var lookingFor = string.Join(", ", profile.LookingFor.Select(l => l.LookingFor));
            sb.AppendLine($"Looking for: {lookingFor}");
        }
        
        if (profile.HasStartup)
        {
            if (!string.IsNullOrWhiteSpace(profile.StartupName))
                sb.AppendLine($"Startup: {profile.StartupName}");
            if (!string.IsNullOrWhiteSpace(profile.StartupDescription))
                sb.AppendLine($"Startup Description: {profile.StartupDescription}");
            if (!string.IsNullOrWhiteSpace(profile.StartupStage))
                sb.AppendLine($"Stage: {profile.StartupStage}");
        }
        
        if (!string.IsNullOrWhiteSpace(profile.CanHelp))
            sb.AppendLine($"Can help: {profile.CanHelp}");
        
        if (!string.IsNullOrWhiteSpace(profile.NeedsHelp))
            sb.AppendLine($"Needs help: {profile.NeedsHelp}");
        
        if (!string.IsNullOrWhiteSpace(profile.AiUsage))
            sb.AppendLine($"AI Usage: {profile.AiUsage}");
        
        return sb.ToString().Trim();
    }
}

