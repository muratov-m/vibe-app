namespace VibeApp.Core.DTOs;

/// <summary>
/// Result of batch import operation
/// </summary>
public class BatchImportResult
{
    public int TotalProcessed { get; set; }
    public int Created { get; set; }
    public int Updated { get; set; }
    public int Deleted { get; set; }
    public List<string> Errors { get; set; } = new();
}



