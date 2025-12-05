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
    
    // Custom fields
    public string Custom_1 { get; set; } = string.Empty;
    public string Custom_2 { get; set; } = string.Empty;
    public string Custom_3 { get; set; } = string.Empty;
    public string Custom_4 { get; set; } = string.Empty;
    public string Custom_5 { get; set; } = string.Empty;
    public string Custom_6 { get; set; } = string.Empty;
    public string Custom_7 { get; set; } = string.Empty;
    
    // Custom arrays
    public List<string> Custom_array_1 { get; set; } = new();
    public List<string> Custom_array_2 { get; set; } = new();
    public List<string> Custom_array_3 { get; set; } = new();
    public List<string> Custom_array_4 { get; set; } = new();
    public List<string> Custom_array_5 { get; set; } = new();
    public List<string> Custom_array_6 { get; set; } = new();
    public List<string> Custom_array_7 { get; set; } = new();
}

