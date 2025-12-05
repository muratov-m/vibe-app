namespace VibeApp.Core.Entities;

/// <summary>
/// User skill entity
/// </summary>
public class UserSkill : IEntity
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public string Skill { get; set; } = string.Empty;
    
    public UserProfile UserProfile { get; set; } = null!;
}

