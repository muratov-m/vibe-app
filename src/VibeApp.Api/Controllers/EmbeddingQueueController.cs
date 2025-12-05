using Microsoft.AspNetCore.Mvc;
using VibeApp.Core.Interfaces;

namespace VibeApp.Api.Controllers;

[ApiController]
[Route("api/embedding-queue")]
public class EmbeddingQueueController : ControllerBase
{
    private readonly IEmbeddingQueueService _queueService;
    private readonly ILogger<EmbeddingQueueController> _logger;

    public EmbeddingQueueController(
        IEmbeddingQueueService queueService,
        ILogger<EmbeddingQueueController> logger)
    {
        _queueService = queueService;
        _logger = logger;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus(CancellationToken cancellationToken)
    {
        try
        {
            var count = await _queueService.GetQueueCountAsync(cancellationToken);
            
            return Ok(new
            {
                profilesInQueue = count,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue status");
            return StatusCode(500, new { error = "Failed to get queue status" });
        }
    }

    [HttpPost("clear")]
    public async Task<IActionResult> ClearQueue(CancellationToken cancellationToken)
    {
        try
        {
            var countBefore = await _queueService.GetQueueCountAsync(cancellationToken);
            await _queueService.ClearQueueAsync(cancellationToken);
            
            _logger.LogInformation("Queue cleared. Removed {Count} items", countBefore);
            
            return Ok(new
            {
                message = "Queue cleared successfully",
                itemsRemoved = countBefore,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing queue");
            return StatusCode(500, new { error = "Failed to clear queue" });
        }
    }
}

