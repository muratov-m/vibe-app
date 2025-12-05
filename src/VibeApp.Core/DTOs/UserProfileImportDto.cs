namespace VibeApp.Core.DTOs;

/// <summary>
/// DTO for importing user profile data from JSON
/// </summary>
public class UserProfileImportDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Telegram { get; set; } = string.Empty;
    public string Linkedin { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
    public bool HasStartup { get; set; }
    public string StartupStage { get; set; } = string.Empty;
    public string StartupDescription { get; set; } = string.Empty;
    public string StartupName { get; set; } = string.Empty;
    public List<string> LookingFor { get; set; } = new();
    public string CanHelp { get; set; } = string.Empty;
    public string NeedsHelp { get; set; } = string.Empty;
    public string AiUsage { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
}

