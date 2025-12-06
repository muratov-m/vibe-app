using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using VibeApp.Core.DTOs;
using VibeApp.Core.Entities;
using VibeApp.Core.Interfaces;

namespace VibeApp.Core.Services;

/// <summary>
/// Service for parsing user profiles using AI and updating structured fields
/// </summary>
public class UserProfileParsingService : IUserProfileParsingService
{
    private readonly IRepository<UserProfile> _userProfileRepository;
    private readonly IOpenAIGateway _openAIGateway;
    private readonly ILogger<UserProfileParsingService> _logger;

    public UserProfileParsingService(
        IRepository<UserProfile> userProfileRepository,
        IOpenAIGateway openAIGateway,
        ILogger<UserProfileParsingService> logger)
    {
        _userProfileRepository = userProfileRepository;
        _openAIGateway = openAIGateway;
        _logger = logger;
    }

    public async Task ParseAndUpdateProfileAsync(int userProfileId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Load profile with related data (Skills and LookingFor)
            var profile = await _userProfileRepository.GetQueryable()
                .Include(p => p.Skills)
                .Include(p => p.LookingFor)
                .FirstOrDefaultAsync(p => p.Id == userProfileId, cancellationToken);
            
            if (profile == null)
            {
                _logger.LogWarning("User profile {UserProfileId} not found for parsing", userProfileId);
                return;
            }

            var prompt = BuildParsingPrompt(profile);
            
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(@"You are a profile data extraction assistant. 
Your task is to analyze user profile information and extract structured data.
Always respond with valid JSON only, no additional text.

Response format:
{
  ""shortBio"": ""Brief summary up to 300 characters"",
  ""mainActivity"": ""Main occupation or role"",
  ""interests"": [""interest1"", ""interest2""],
  ""country"": ""Country name"",
  ""city"": ""City name""
}

Rules:
- shortBio: Summarize the most important aspects, keep it under 300 characters
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
            _logger.LogError(ex, "Error parsing profile {UserProfileId} with OpenAI", userProfileId);
            throw;
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

