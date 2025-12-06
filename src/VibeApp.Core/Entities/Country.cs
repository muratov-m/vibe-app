namespace VibeApp.Core.Entities;

/// <summary>
/// Reference table for countries extracted from user profiles
/// </summary>
public class Country : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

