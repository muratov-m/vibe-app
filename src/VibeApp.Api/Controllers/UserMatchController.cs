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
    private readonly ILogger<UserMatchController> _logger;

    public UserMatchController(
        IUserMatchingEmbeddingService matchingService,
        ILogger<UserMatchController> logger)
    {
        _matchingService = matchingService;
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

            _logger.LogInformation("User match request: MainActivity={MainActivity}, Interests={Interests}, Country={Country}, City={City}, TopK={TopK}",
                request.MainActivity, request.Interests, request.Country, request.City, request.TopK);

            var matchingUsers = await _matchingService.FindMatchingUsersAsync(
                request.MainActivity,
                request.Interests,
                request.Country,
                request.City,
                request.TopK,
                cancellationToken);

            var response = matchingUsers.Select(match => new UserMatchResponseDto
            {
                Similarity = match.Similarity,
                Profile = new UserProfileResponseDto
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
                }
            }).ToList();

            _logger.LogInformation("Found {Count} matching users", response.Count);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding matching users");
            return StatusCode(500, new { error = "Failed to find matching users", details = ex.Message });
        }
    }
}

