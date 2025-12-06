using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector;
using Pgvector.EntityFrameworkCore;
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
        return BuildMatchingText(
            profile.ParsedMainActivity,
            profile.ParsedInterests,
            profile.ParsedCountry,
            profile.ParsedCity
        );
    }
    
    /// <summary>
    /// Builds matching text from provided fields
    /// </summary>
    private static string BuildMatchingText(string? mainActivity, string? interests, string? country, string? city)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(mainActivity))
            sb.AppendLine($"Main activity: {mainActivity}");

        if (!string.IsNullOrWhiteSpace(interests))
            sb.AppendLine($"Interests: {interests}");

        if (!string.IsNullOrWhiteSpace(country))
            sb.AppendLine($"Country: {country}");
        
        if (!string.IsNullOrWhiteSpace(city))
            sb.AppendLine($"City: {city}");

        return sb.ToString().Trim();
    }
    
    public async Task<List<(UserProfile Profile, float Similarity)>> FindMatchingUsersAsync(
        string mainActivity, 
        string interests, 
        string? country, 
        string? city, 
        int topK = 3, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Build text from input
            var matchingText = BuildMatchingText(mainActivity, interests, country, city);
            
            if (string.IsNullOrWhiteSpace(matchingText))
            {
                _logger.LogWarning("No matching text generated from input");
                return new List<(UserProfile, float)>();
            }

            // Generate embedding from input
            var embeddingArray = await _openAIGateway.GetEmbeddingAsync(matchingText, cancellationToken: cancellationToken);
            var searchVector = new Vector(embeddingArray);

            // Search for similar users using cosine similarity
            // Formula: similarity = 1 - cosine_distance
            var matchingUsers = await _embeddingRepository.GetQueryable()
                .Include(e => e.UserProfile)
                .Select(e => new
                {
                    e.UserProfile,
                    Similarity = 1 - e.Embedding.CosineDistance(searchVector)
                })
                .OrderByDescending(x => x.Similarity)
                .Take(topK)
                .ToListAsync(cancellationToken);

            var results = matchingUsers
                .Select(x => (x.UserProfile!, (float)x.Similarity))
                .ToList();

            _logger.LogInformation("Found {Count} matching users", results.Count);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding matching users");
            throw;
        }
    }
}

