namespace VibeApp.Core.Entities;

/// <summary>
/// What user is looking for (networking preferences)
/// </summary>
public class UserLookingFor : IEntity
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public string LookingFor { get; set; } = string.Empty;
    
    public UserProfile UserProfile { get; set; } = null!;
}

