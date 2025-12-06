namespace VibeApp.Core.DTOs;

/// <summary>
/// Response DTO for user matching
/// </summary>
public class UserMatchResponseDto
{
    /// <summary>
    /// Matching user profile
    /// </summary>
    public UserProfileResponseDto Profile { get; set; } = null!;
    
    /// <summary>
    /// Similarity score (0-1, higher is better match)
    /// </summary>
    public float Similarity { get; set; }
}

/// <summary>
/// User profile information for matching results
/// </summary>
public class UserProfileResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? MainActivity { get; set; }
    public string? Interests { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public bool HasStartup { get; set; }
    public string? StartupDescription { get; set; }
}

