namespace VibeApp.Core.Entities;

/// <summary>
/// Custom array 7 items
/// </summary>
public class UserCustomArray7 : IEntity
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public string Value { get; set; } = string.Empty;
    
    public UserProfile UserProfile { get; set; } = null!;
}

