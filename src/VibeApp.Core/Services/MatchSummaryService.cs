using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using VibeApp.Core.DTOs;
using VibeApp.Core.Interfaces;

namespace VibeApp.Core.Services;

/// <summary>
/// Service for generating AI-powered match summaries and starter messages
/// </summary>
public class MatchSummaryService : IMatchSummaryService
{
    /// <summary>
    /// Maximum number of profiles to process with AI
    /// </summary>
    private const int MaxProfilesForAi = 3;
    
    private readonly IOpenAIGateway _openAiGateway;
    private readonly ILogger<MatchSummaryService> _logger;

    public MatchSummaryService(
        IOpenAIGateway openAiGateway,
        ILogger<MatchSummaryService> logger)
    {
        _openAiGateway = openAiGateway;
        _logger = logger;
    }

    public async Task<Dictionary<int, MatchProfileSummary>> GenerateBatchSummariesAsync(
        UserMatchRequestDto searchRequest,
        List<UserProfileResponseDto> matchedProfiles,
        CancellationToken cancellationToken = default)
    {
        var profilesToProcess = matchedProfiles
            .Take(MaxProfilesForAi)
            .ToList();

        if (profilesToProcess.Count == 0)
        {
            return new Dictionary<int, MatchProfileSummary>();
        }

        try
        {
            var systemPrompt = @"Ты - AI ассистент для платформы нетворкинга.
Твоя задача - для каждого профиля создать:
1. Краткое саммари (1-2 предложения), объясняющее, почему этот профиль подходит пользователю
2. Дружелюбное стартовое сообщение для начала диалога (2-3 предложения)

Стиль саммари:
- Кратко, персонализированно, дружелюбно
- Используй имя найденного человека и конкретные детали
- Примеры: ""Nikita ищет React-разработчика — это про тебя!"", ""С Maksim у вас общие интересы: AI и Machine Learning""

Стиль стартового сообщения:
- От первого лица, как будто это пишет сам пользователь
- Упоминать общие интересы или деятельность
- Тон: дружелюбный, неформальный, но профессиональный
- Примеры: ""Привет! Вижу, ты тоже увлекаешься React-разработкой. Было бы круто обменяться опытом!""

ВАЖНО: Отвечай ТОЛЬКО валидным JSON массивом без дополнительных пояснений.
Формат: [{""profileId"": 123, ""summary"": ""текст саммари"", ""starterMessage"": ""текст сообщения""}]";

            // Estimate capacity: ~200 chars per profile + header
            var estimatedCapacity = 300 + (profilesToProcess.Count * 200);
            var profilesInfo = new StringBuilder(estimatedCapacity);
            
            profilesInfo.AppendLine($"Критерии поиска пользователя:");
            profilesInfo.AppendLine($"- Основная деятельность: {searchRequest.MainActivity}");
            profilesInfo.AppendLine($"- Интересы: {searchRequest.Interests}");
            profilesInfo.AppendLine($"- Страна: {searchRequest.Country ?? "не указана"}");
            profilesInfo.AppendLine($"- Город: {searchRequest.City ?? "не указан"}");
            profilesInfo.AppendLine();
            profilesInfo.AppendLine("Найденные профили:");
            profilesInfo.AppendLine();

            foreach (var profile in profilesToProcess)
            {
                profilesInfo.AppendLine($"Profile ID: {profile.Id}");
                profilesInfo.AppendLine($"- Имя: {profile.Name}");
                profilesInfo.AppendLine($"- Основная деятельность: {profile.MainActivity ?? "не указана"}");
                profilesInfo.AppendLine($"- Интересы: {profile.Interests ?? "не указаны"}");
                profilesInfo.AppendLine($"- Страна: {profile.Country ?? "не указана"}");
                profilesInfo.AppendLine($"- Город: {profile.City ?? "не указан"}");
                profilesInfo.AppendLine($"- Биография: {profile.Bio ?? "не указана"}");
                profilesInfo.AppendLine($"- Имеет стартап: {(profile.HasStartup ? $"Да ({profile.StartupDescription})" : "Нет")}");
                profilesInfo.AppendLine();
            }

            profilesInfo.AppendLine("Создай для каждого профиля саммари и стартовое сообщение в формате JSON.");

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(profilesInfo.ToString())
            };

            _logger.LogInformation("Generating batch summaries for {Count} profiles", profilesToProcess.Count);

            var response = await _openAiGateway.CreateChatCompletionAsync(
                messages,
                model: "gpt-4.1-nano",
                temperature: 0.7f,
                maxTokens: 500,
                cancellationToken: cancellationToken);

            // Parse JSON response
            var jsonResponse = CleanJsonResponse(response);

            var summaries = JsonSerializer.Deserialize<List<MatchProfileSummary>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (summaries == null || summaries.Count == 0)
            {
                _logger.LogWarning("Failed to parse AI response, using fallback summaries");
                return CreateFallbackSummaries(profilesToProcess);
            }

            var result = new Dictionary<int, MatchProfileSummary>();
            foreach (var summary in summaries)
            {
                if (summary.ProfileId > 0)
                {
                    result[summary.ProfileId] = summary;
                }
            }

            _logger.LogInformation("Successfully generated {Count} summaries", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating batch summaries");
            return CreateFallbackSummaries(profilesToProcess);
        }
    }

    private Dictionary<int, MatchProfileSummary> CreateFallbackSummaries(List<UserProfileResponseDto> profiles)
    {
        var result = new Dictionary<int, MatchProfileSummary>();
        
        foreach (var profile in profiles)
        {
            result[profile.Id] = new MatchProfileSummary
            {
                ProfileId = profile.Id,
                Summary = $"{profile.Name} подходит вам по критериям поиска.",
                StarterMessage = "Привет! Вижу, у нас есть общие интересы. Было бы интересно пообщаться!"
            };
        }

        return result;
    }

    /// <summary>
    /// Safely removes markdown code block formatting from JSON response
    /// </summary>
    private string CleanJsonResponse(string response)
    {
        var cleaned = response.Trim();
        
        // Remove opening markdown code blocks
        if (cleaned.StartsWith("```json", StringComparison.OrdinalIgnoreCase) && cleaned.Length > 7)
        {
            cleaned = cleaned.Substring(7);
        }
        else if (cleaned.StartsWith("```") && cleaned.Length > 3)
        {
            cleaned = cleaned.Substring(3);
        }
        
        // Remove closing markdown code blocks
        if (cleaned.EndsWith("```") && cleaned.Length > 3)
        {
            cleaned = cleaned.Substring(0, cleaned.Length - 3);
        }
        
        return cleaned.Trim();
    }
}
