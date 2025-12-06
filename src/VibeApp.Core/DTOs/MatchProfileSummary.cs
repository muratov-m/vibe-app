namespace VibeApp.Core.DTOs;

/// <summary>
/// DTO for AI-generated match profile summary and starter message
/// </summary>
public class MatchProfileSummary
{
    /// <summary>
    /// Profile ID
    /// </summary>
    public int ProfileId { get; set; }
    
    /// <summary>
    /// AI-generated summary explaining why this profile matched
    /// </summary>
    public string Summary { get; set; } = string.Empty;
    
    /// <summary>
    /// AI-generated starter message suggestion
    /// </summary>
    public string StarterMessage { get; set; } = string.Empty;
}

