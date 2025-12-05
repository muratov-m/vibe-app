namespace VibeApp.Core.Entities;

/// <summary>
/// User profile entity containing professional information and networking preferences
/// </summary>
public class UserProfile : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Telegram { get; set; } = string.Empty;
    public string LinkedIn { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    
    // Startup information
    public bool HasStartup { get; set; }
    public string StartupStage { get; set; } = string.Empty;
    public string StartupDescription { get; set; } = string.Empty;
    public string StartupName { get; set; } = string.Empty;
    
    // Help and collaboration
    public string CanHelp { get; set; } = string.Empty;
    public string NeedsHelp { get; set; } = string.Empty;
    public string AiUsage { get; set; } = string.Empty;
    
    // Custom extensibility fields
    public string Custom1 { get; set; } = string.Empty;
    public string Custom2 { get; set; } = string.Empty;
    public string Custom3 { get; set; } = string.Empty;
    public string Custom4 { get; set; } = string.Empty;
    public string Custom5 { get; set; } = string.Empty;
    public string Custom6 { get; set; } = string.Empty;
    public string Custom7 { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<UserSkill> Skills { get; set; } = new List<UserSkill>();
    public ICollection<UserLookingFor> LookingFor { get; set; } = new List<UserLookingFor>();
    public ICollection<UserCustomArray1> CustomArray1 { get; set; } = new List<UserCustomArray1>();
    public ICollection<UserCustomArray2> CustomArray2 { get; set; } = new List<UserCustomArray2>();
    public ICollection<UserCustomArray3> CustomArray3 { get; set; } = new List<UserCustomArray3>();
    public ICollection<UserCustomArray4> CustomArray4 { get; set; } = new List<UserCustomArray4>();
    public ICollection<UserCustomArray5> CustomArray5 { get; set; } = new List<UserCustomArray5>();
    public ICollection<UserCustomArray6> CustomArray6 { get; set; } = new List<UserCustomArray6>();
    public ICollection<UserCustomArray7> CustomArray7 { get; set; } = new List<UserCustomArray7>();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
