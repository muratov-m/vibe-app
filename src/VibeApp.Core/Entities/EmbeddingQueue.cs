namespace VibeApp.Core.Entities;

public class EmbeddingQueue : IEntity
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public DateTime CreatedAt { get; set; }
}

