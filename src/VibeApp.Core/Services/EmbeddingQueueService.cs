using Microsoft.EntityFrameworkCore;
using VibeApp.Core.Entities;
using VibeApp.Core.Interfaces;

namespace VibeApp.Core.Services;

public class EmbeddingQueueService : IEmbeddingQueueService
{
    private readonly IRepository<EmbeddingQueue> _queueRepository;

    public EmbeddingQueueService(IRepository<EmbeddingQueue> queueRepository)
    {
        _queueRepository = queueRepository;
    }

    public async Task EnqueueProfileAsync(int userProfileId, CancellationToken cancellationToken = default)
    {
        // Check if profile is already in queue
        var existingItem = await _queueRepository.GetQueryable()
            .FirstOrDefaultAsync(q => q.UserProfileId == userProfileId, cancellationToken);

        if (existingItem == null)
        {
            var queueItem = new EmbeddingQueue
            {
                UserProfileId = userProfileId,
                CreatedAt = DateTime.UtcNow
            };

            await _queueRepository.AddAsync(queueItem);
        }
    }

    public async Task<List<(int QueueItemId, int UserProfileId)>> DequeueBatchAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        // Filter items with retry count less than 3
        var queueItems = await _queueRepository.GetQueryable()
            .Where(q => q.RetryCount < 3)
            .OrderBy(q => q.CreatedAt)
            .Take(batchSize)
            .Select(q => new { q.Id, q.UserProfileId })
            .ToListAsync(cancellationToken);

        return queueItems.Select(q => (q.Id, q.UserProfileId)).ToList();
    }

    public async Task RemoveFromQueueAsync(int queueItemId, CancellationToken cancellationToken = default)
    {
        await _queueRepository.DeleteAsync(queueItemId);
    }

    public async Task RequeueWithRetryAsync(int queueItemId, CancellationToken cancellationToken = default)
    {
        var queueItem = await _queueRepository.GetQueryable()
            .FirstOrDefaultAsync(q => q.Id == queueItemId, cancellationToken);

        if (queueItem != null)
        {
            queueItem.RetryCount++;
            queueItem.LastProcessedAt = DateTime.UtcNow;
            queueItem.CreatedAt = DateTime.UtcNow; // Move to end of queue
            
            await _queueRepository.UpdateAsync(queueItem);
        }
    }

    public async Task<int> GetQueueCountAsync(CancellationToken cancellationToken = default)
    {
        return await _queueRepository.GetQueryable()
            .CountAsync(cancellationToken);
    }

    public async Task ClearQueueAsync(CancellationToken cancellationToken = default)
    {
        var allItems = await _queueRepository.GetQueryable().ToListAsync(cancellationToken);
        
        foreach (var item in allItems)
        {
            await _queueRepository.DeleteAsync(item);
        }
    }
}

