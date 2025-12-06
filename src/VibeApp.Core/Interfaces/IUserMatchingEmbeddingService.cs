using VibeApp.Core.Entities;

namespace VibeApp.Core.Interfaces;

/// <summary>
/// Service for managing user matching embeddings
/// </summary>
public interface IUserMatchingEmbeddingService
{
    /// <summary>
    /// Generates and saves matching embedding for a user profile
    /// Uses ParsedInterests, ParsedMainActivity, ParsedCountry, ParsedCity
    /// </summary>
    Task GenerateAndSaveEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes matching embedding for a user profile
    /// </summary>
    Task DeleteEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets matching embedding for a user profile
    /// </summary>
    Task<UserMatchingEmbedding?> GetEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default);
}

