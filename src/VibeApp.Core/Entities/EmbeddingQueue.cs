namespace VibeApp.Core.Entities;

public class EmbeddingQueue : IEntity
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RetryCount { get; set; } = 0;
    public DateTime? LastProcessedAt { get; set; }
}

