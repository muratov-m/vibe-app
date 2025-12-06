namespace VibeApp.Core.DTOs;

/// <summary>
/// Request DTO for RAG search
/// </summary>
public class RagSearchRequestDto
{
    /// <summary>
    /// User's natural language query (e.g., "Who here knows Rust and likes hiking?")
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of results to return (default: 5)
    /// </summary>
    public int TopK { get; set; } = 5;

    /// <summary>
    /// Whether to generate a natural language response using LLM (default: true)
    /// </summary>
    public bool GenerateResponse { get; set; } = true;

    /// <summary>
    /// Optional filters for hybrid search (semantic + structured)
    /// </summary>
    public UserSearchFilters? Filters { get; set; }
}

