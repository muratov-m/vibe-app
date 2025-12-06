using Microsoft.AspNetCore.Mvc;
using VibeApp.Core.DTOs;
using VibeApp.Core.Interfaces;

namespace VibeApp.Api.Controllers;

/// <summary>
/// Controller for RAG (Retrieval Augmented Generation) search across user profiles
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RagSearchController : ControllerBase
{
    private readonly IRagSearchService _ragSearchService;
    private readonly ILogger<RagSearchController> _logger;

    public RagSearchController(IRagSearchService ragSearchService, ILogger<RagSearchController> logger)
    {
        _ragSearchService = ragSearchService;
        _logger = logger;
    }

    /// <summary>
    /// Search for user profiles using natural language query
    /// </summary>
    /// <remarks>
    /// Example queries:
    /// - "Who here knows Rust and likes hiking?"
    /// - "Find me someone who can help with machine learning"
    /// - "Looking for a co-founder with marketing experience"
    /// - "Who has experience with startups in the AI space?"
    /// </remarks>
    /// <param name="request">Search request with query and options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching profiles with optional AI-generated response</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(RagSearchResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RagSearchResponseDto>> Search(
        [FromBody] RagSearchRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest(new { error = "Query cannot be empty" });
            }

            if (request.TopK < 1 || request.TopK > 50)
            {
                return BadRequest(new { error = "TopK must be between 1 and 50" });
            }

            _logger.LogInformation("RAG search request: {Query}, TopK: {TopK}, GenerateResponse: {GenerateResponse}",
                request.Query, request.TopK, request.GenerateResponse);

            var result = await _ragSearchService.SearchAsync(request, cancellationToken);

            _logger.LogInformation("RAG search completed. Found {Count} results", result.TotalResults);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing RAG search");
            return StatusCode(500, new { error = "Failed to perform search", details = ex.Message });
        }
    }

    /// <summary>
    /// Quick search with GET method (simpler usage)
    /// </summary>
    /// <param name="q">Search query</param>
    /// <param name="top">Number of results (default: 5)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching profiles with AI-generated response</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(RagSearchResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RagSearchResponseDto>> SearchGet(
        [FromQuery] string q,
        [FromQuery] int top = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { error = "Query parameter 'q' is required" });
        }

        var request = new RagSearchRequestDto
        {
            Query = q,
            TopK = Math.Clamp(top, 1, 50),
            GenerateResponse = true
        };

        return await Search(request, cancellationToken);
    }
}

