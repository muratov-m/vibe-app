using System.ComponentModel.DataAnnotations;

namespace VibeApp.Core.DTOs;

/// <summary>
/// Request DTO for finding matching users
/// </summary>
public class UserMatchRequestDto
{
    /// <summary>
    /// Main activity/occupation of the user
    /// </summary>
    [Required]
    public string MainActivity { get; set; } = string.Empty;
    
    /// <summary>
    /// Comma-separated list of interests
    /// </summary>
    [Required]
    public string Interests { get; set; } = string.Empty;
    
    /// <summary>
    /// Country (optional)
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// City (optional)
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Number of matching users to return (default: 3)
    /// </summary>
    public int TopK { get; set; } = 3;
}

