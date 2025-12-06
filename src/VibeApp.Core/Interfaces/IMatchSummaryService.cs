using VibeApp.Core.DTOs;

namespace VibeApp.Core.Interfaces;

/// <summary>
/// Service for generating AI-powered match summaries and starter messages
/// </summary>
public interface IMatchSummaryService
{
    /// <summary>
    /// Generates AI summaries and starter messages for multiple profiles in a single batch request
    /// </summary>
    /// <param name="searchRequest">The original search request criteria</param>
    /// <param name="matchedProfiles">List of matched user profiles (max 3 will be processed)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary mapping profile ID to AI-generated summary and starter message</returns>
    Task<Dictionary<int, MatchProfileSummary>> GenerateBatchSummariesAsync(
        UserMatchRequestDto searchRequest,
        List<UserProfileResponseDto> matchedProfiles,
        CancellationToken cancellationToken = default);
}

