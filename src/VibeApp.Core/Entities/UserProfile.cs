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
    
    // Parsed structured data (extracted by AI)
    public string ParsedShortBio { get; set; } = string.Empty;
    public string ParsedMainActivity { get; set; } = string.Empty;
    public string ParsedInterests { get; set; } = string.Empty;
    public string ParsedCountry { get; set; } = string.Empty;
    public string ParsedCity { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<UserSkill> Skills { get; set; } = new List<UserSkill>();
    public ICollection<UserLookingFor> LookingFor { get; set; } = new List<UserLookingFor>();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
