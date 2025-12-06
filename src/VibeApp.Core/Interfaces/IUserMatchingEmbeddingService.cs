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
    
    /// <summary>
    /// Finds matching users based on provided user criteria (mainActivity, interests, country, city)
    /// Generates embedding from input and searches for similar users using cosine similarity
    /// </summary>
    /// <param name="mainActivity">Main activity/occupation</param>
    /// <param name="interests">Comma-separated list of interests</param>
    /// <param name="country">Country</param>
    /// <param name="city">City</param>
    /// <param name="topK">Number of results to return (default: 3)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching user profiles with similarity scores</returns>
    Task<List<(UserProfile Profile, float Similarity)>> FindMatchingUsersAsync(
        string mainActivity, 
        string interests, 
        string? country, 
        string? city, 
        int topK = 3, 
        CancellationToken cancellationToken = default);
}

