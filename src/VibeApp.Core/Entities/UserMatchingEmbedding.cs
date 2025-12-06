using Pgvector;

namespace VibeApp.Core.Entities;

/// <summary>
/// Entity for storing user matching embeddings (vector representations for matching purposes)
/// Based on structured fields: ParsedInterests, ParsedMainActivity, ParsedCountry, ParsedCity
/// </summary>
public class UserMatchingEmbedding : IEntity
{
    public int Id { get; set; }
    
    /// <summary>
    /// Foreign key to UserProfile
    /// </summary>
    public int UserProfileId { get; set; }
    
    /// <summary>
    /// Vector embedding for matching (1536 dimensions for OpenAI embeddings)
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


