using Pgvector;

namespace VibeApp.Core.Entities;

/// <summary>
/// Entity for storing user profile embeddings (vector representations)
/// </summary>
public class UserProfileEmbedding : IEntity
{
    public int Id { get; set; }
    
    /// <summary>
    /// Foreign key to UserProfile
    /// </summary>
    public int UserProfileId { get; set; }
    
    /// <summary>
    /// Vector embedding of the user profile (1536 dimensions for OpenAI embeddings)
    /// </summary>
    public Vector Embedding { get; set; } = null!;
    
    /// <summary>
    /// When the embedding was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the embedding was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to UserProfile
    /// </summary>
    public UserProfile UserProfile { get; set; } = null!;
}

