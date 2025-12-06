using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector;
using VibeApp.Core.Entities;
using VibeApp.Core.Interfaces;

namespace VibeApp.Core.Services;

/// <summary>
/// Service for managing user matching embeddings
/// </summary>
public class UserMatchingEmbeddingService : IUserMatchingEmbeddingService
{
    private readonly IRepository<UserProfile> _userProfileRepository;
    private readonly IRepository<UserMatchingEmbedding> _embeddingRepository;
    private readonly IOpenAIGateway _openAIGateway;
    private readonly ILogger<UserMatchingEmbeddingService> _logger;

    public UserMatchingEmbeddingService(
        IRepository<UserProfile> userProfileRepository,
        IRepository<UserMatchingEmbedding> embeddingRepository,
        IOpenAIGateway openAIGateway,
        ILogger<UserMatchingEmbeddingService> logger)
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
            _logger.LogInformation("Generating matching embedding for user profile {UserProfileId}", userProfileId);

            // Get user profile
            var profile = await _userProfileRepository.GetQueryable()
                .FirstOrDefaultAsync(p => p.Id == userProfileId, cancellationToken);
            
            if (profile == null)
            {
                _logger.LogWarning("User profile {UserProfileId} not found", userProfileId);
                return;
            }

            // Check if profile has required parsed fields
            if (string.IsNullOrWhiteSpace(profile.ParsedInterests) &&
                string.IsNullOrWhiteSpace(profile.ParsedMainActivity) &&
                string.IsNullOrWhiteSpace(profile.ParsedCountry) &&
                string.IsNullOrWhiteSpace(profile.ParsedCity))
            {
                _logger.LogWarning("User profile {UserProfileId} has no parsed fields for matching embedding", userProfileId);
                return;
            }

            // Generate embedding
            var embedding = await GenerateMatchingEmbeddingAsync(profile, cancellationToken);

            // Check if embedding already exists
            var existingEmbedding = await _embeddingRepository.FirstOrDefaultAsync(e => e.UserProfileId == userProfileId);

            if (existingEmbedding != null)
            {
                // Update existing embedding
                existingEmbedding.Embedding = embedding;
                existingEmbedding.UpdatedAt = DateTime.UtcNow;
                await _embeddingRepository.UpdateAsync(existingEmbedding);
                _logger.LogInformation("Updated matching embedding for user profile {UserProfileId}", userProfileId);
            }
            else
            {
                // Create new embedding
                var newEmbedding = new UserMatchingEmbedding
                {
                    UserProfileId = userProfileId,
                    Embedding = embedding,
                    CreatedAt = DateTime.UtcNow
                };
                await _embeddingRepository.AddAsync(newEmbedding);
                _logger.LogInformation("Created new matching embedding for user profile {UserProfileId}", userProfileId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating matching embedding for user profile {UserProfileId}", userProfileId);
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
                _logger.LogInformation("Deleted matching embedding for user profile {UserProfileId}", userProfileId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting matching embedding for user profile {UserProfileId}", userProfileId);
            throw;
        }
    }

    public async Task<UserMatchingEmbedding?> GetEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default)
    {
        return await _embeddingRepository.FirstOrDefaultAsync(e => e.UserProfileId == userProfileId);
    }

    private async Task<Vector> GenerateMatchingEmbeddingAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        var matchingText = BuildMatchingText(profile);
        _logger.LogInformation("Generating matching embedding for profile: {Name}, text length: {Length}", 
            profile.Name, matchingText.Length);

        var embeddingArray = await _openAIGateway.GetEmbeddingAsync(matchingText, cancellationToken: cancellationToken);
        
        return new Vector(embeddingArray);
    }

    /// <summary>
    /// Builds matching text from parsed fields only:
    /// ParsedInterests, ParsedMainActivity, ParsedCountry, ParsedCity
    /// </summary>
    private static string BuildMatchingText(UserProfile profile)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(profile.ParsedInterests))
            sb.AppendLine($"Interests: {profile.ParsedInterests}");
        
        if (!string.IsNullOrWhiteSpace(profile.ParsedMainActivity))
            sb.AppendLine($"Main activity: {profile.ParsedMainActivity}");
        
        if (!string.IsNullOrWhiteSpace(profile.ParsedCountry))
            sb.AppendLine($"Country: {profile.ParsedCountry}");
        
        if (!string.IsNullOrWhiteSpace(profile.ParsedCity))
            sb.AppendLine($"City: {profile.ParsedCity}");

        return sb.ToString().Trim();
    }
}

