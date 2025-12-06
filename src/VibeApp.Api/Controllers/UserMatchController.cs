using Microsoft.AspNetCore.Mvc;
using VibeApp.Core.DTOs;
using VibeApp.Core.Interfaces;

namespace VibeApp.Api.Controllers;

/// <summary>
/// Controller for finding matching users based on user preferences
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserMatchController : ControllerBase
{
    private readonly IUserMatchingEmbeddingService _matchingService;
    private readonly IMatchSummaryService _matchSummaryService;
    private readonly ILogger<UserMatchController> _logger;

    public UserMatchController(
        IUserMatchingEmbeddingService matchingService,
        IMatchSummaryService matchSummaryService,
        ILogger<UserMatchController> logger)
    {
        _matchingService = matchingService;
        _matchSummaryService = matchSummaryService;
        _logger = logger;
    }

    /// <summary>
    /// Find matching users based on provided criteria
    /// </summary>
    /// <remarks>
    /// Finds users that match the provided main activity, interests, country, and city.
    /// Uses AI embeddings and cosine similarity to find the best matches.
    /// 
    /// Example request:
    /// ```json
    /// {
    ///   "mainActivity": "Software Developer",
    ///   "interests": "AI, Machine Learning, Hiking",
    ///   "country": "Germany",
    ///   "city": "Berlin",
    ///   "topK": 3
    /// }
    /// ```
    /// </remarks>
    /// <param name="request">User matching request with criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching users with similarity scores</returns>
    [HttpPost("match")]
    [ProducesResponseType(typeof(List<UserMatchResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<UserMatchResponseDto>>> Match(
        [FromBody] UserMatchRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.MainActivity))
            {
                return BadRequest(new { error = "MainActivity is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Interests))
            {
                return BadRequest(new { error = "Interests are required" });
            }

            if (request.TopK < 1 || request.TopK > 20)
            {
                return BadRequest(new { error = "TopK must be between 1 and 20" });
            }

            var matchingUsers = await _matchingService.FindMatchingUsersAsync(
                request.MainActivity,
                request.Interests,
                request.Country,
                request.City,
                request.TopK,
                cancellationToken);

            // Convert matched users to UserProfileResponseDto list
            var profileDtos = matchingUsers
                .OrderByDescending(x => x.Similarity)
                .Select(match => new UserProfileResponseDto
                {
                    Id = match.Profile.Id,
                    Name = match.Profile.Name,
                    Bio = match.Profile.Bio,
                    MainActivity = match.Profile.ParsedMainActivity,
                    Interests = match.Profile.ParsedInterests,
                    Country = match.Profile.ParsedCountry,
                    City = match.Profile.ParsedCity,
                    HasStartup = match.Profile.HasStartup,
                    StartupDescription = match.Profile.StartupDescription
                }).ToList();

            // Generate AI summaries and starter messages for all profiles in a single batch request
            var aiSummaries = await _matchSummaryService.GenerateBatchSummariesAsync(
                request, 
                profileDtos, 
                cancellationToken);

            // Build response with AI-generated content
            var response = matchingUsers.Select((match, index) =>
            {
                var profileDto = profileDtos[index];
                var hasSummary = aiSummaries.TryGetValue(profileDto.Id, out var summary);

                return new UserMatchResponseDto
                {
                    Similarity = match.Similarity,
                    Profile = profileDto,
                    AiSummary = hasSummary ? summary.Summary : $"{profileDto.Name} подходит вам по критериям поиска.",
                    StarterMessage = hasSummary ? summary.StarterMessage : "Привет! Вижу, у нас есть общие интересы. Было бы интересно пообщаться!"
                };
            }).ToList();

            _logger.LogInformation("Found {Count} matching users with AI summaries", response.Count);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding matching users");
            return StatusCode(500, new { error = "Failed to find matching users", details = ex.Message });
        }
    }
}

