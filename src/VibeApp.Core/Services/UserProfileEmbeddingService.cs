using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector;
using VibeApp.Core.Entities;
using VibeApp.Core.Interfaces;

namespace VibeApp.Core.Services;

/// <summary>
/// Service for managing user profile embeddings
/// </summary>
public class UserProfileEmbeddingService : IUserProfileEmbeddingService
{
    private readonly IRepository<UserProfile> _userProfileRepository;
    private readonly IRepository<UserProfileEmbedding> _embeddingRepository;
    private readonly ILogger<UserProfileEmbeddingService> _logger;

    public UserProfileEmbeddingService(
        IRepository<UserProfile> userProfileRepository,
        IRepository<UserProfileEmbedding> embeddingRepository,
        ILogger<UserProfileEmbeddingService> logger)
    {
        _userProfileRepository = userProfileRepository;
        _embeddingRepository = embeddingRepository;
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

            // Generate embedding (placeholder for now)
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

    /// <summary>
    /// Generate embedding from user profile
    /// This is a placeholder method - actual implementation will use OpenAI or similar service
    /// </summary>
    private Task<Vector> GenerateEmbeddingAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating embedding for profile: {Name}", profile.Name);

        // TODO: Implement actual embedding generation logic
        // For now, return a zero vector of dimension 1536 (OpenAI embedding size)
        var embeddingArray = new float[1536];
        Array.Fill(embeddingArray, 0f);
        
        // In real implementation, you would:
        // 1. Combine profile text (name, bio, skills, etc.)
        // 2. Call embedding API (e.g., OpenAI)
        // 3. Return the generated embedding
        
        var vector = new Vector(embeddingArray);
        return Task.FromResult(vector);
    }
}

