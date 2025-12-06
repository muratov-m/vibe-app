namespace VibeApp.Core.DTOs;

/// <summary>
/// DTO for parsed user profile information from AI
/// </summary>
public class UserProfileParsedDto
{
    public string ShortBio { get; set; } = string.Empty;
    public string MainActivity { get; set; } = string.Empty;
    public List<string> Interests { get; set; } = new();
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

