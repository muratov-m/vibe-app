namespace VibeApp.Core.Entities;

/// <summary>
/// Custom array 5 items
/// </summary>
public class UserCustomArray5 : IEntity
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public string Value { get; set; } = string.Empty;
    
    public UserProfile UserProfile { get; set; } = null!;
}

