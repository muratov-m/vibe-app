using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VibeApp.Core.Interfaces;

namespace VibeApp.Core.Services;

public class EmbeddingProcessingService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EmbeddingProcessingService> _logger;
    private readonly int _concurrentTasks;

    public EmbeddingProcessingService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<EmbeddingProcessingService> logger,
        IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        
        // Read config value with default
        var configValue = configuration["EmbeddingProcessing:ConcurrentTasks"];
        _concurrentTasks = int.TryParse(configValue, out var value) ? value : 5;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Embedding Processing Service started with {ConcurrentTasks} concurrent tasks", _concurrentTasks);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var queueService = scope.ServiceProvider.GetRequiredService<IEmbeddingQueueService>();

                // Get batch of items from queue
                var batch = await queueService.DequeueBatchAsync(_concurrentTasks, stoppingToken);

                if (batch.Count > 0)
                {
                    _logger.LogInformation("Processing batch of {Count} profiles", batch.Count);

                    // Process all items in parallel
                    var tasks = batch.Select(item => 
                        ProcessEmbeddingAsync(item.QueueItemId, item.UserProfileId, stoppingToken)
                    ).ToList();

                    await Task.WhenAll(tasks);
                }
                else
                {
                    // No items in queue, wait before checking again
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Embedding Processing Service main loop");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("Embedding Processing Service stopped");
    }

    private async Task ProcessEmbeddingAsync(int queueItemId, int userProfileId, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var embeddingService = scope.ServiceProvider.GetRequiredService<IUserProfileEmbeddingService>();
            var queueService = scope.ServiceProvider.GetRequiredService<IEmbeddingQueueService>();

            _logger.LogInformation("Processing embedding for profile ID: {ProfileId} (Queue Item: {QueueItemId})", 
                userProfileId, queueItemId);

            await embeddingService.GenerateAndSaveEmbeddingAsync(userProfileId, cancellationToken);
            
            // Only remove from queue after successful embedding generation
            await queueService.RemoveFromQueueAsync(queueItemId, cancellationToken);
            
            _logger.LogInformation("Successfully generated embedding for profile ID: {ProfileId}", userProfileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for profile ID: {ProfileId}. Item will remain in queue for retry.", 
                userProfileId);
        }
    }
}

