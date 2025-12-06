namespace VibeApp.Core.Interfaces;

/// <summary>
/// Service for managing user profile embeddings
/// </summary>
public interface IUserProfileEmbeddingService
{
    /// <summary>
    /// Generate and save embedding for a user profile
    /// </summary>
    /// <param name="userProfileId">ID of the user profile</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task GenerateAndSaveEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete embedding for a user profile
    /// </summary>
    /// <param name="userProfileId">ID of the user profile</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default);
}

