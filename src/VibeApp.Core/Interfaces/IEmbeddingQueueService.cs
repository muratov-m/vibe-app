namespace VibeApp.Core.Interfaces;

public interface IEmbeddingQueueService
{
    Task EnqueueProfileAsync(int userProfileId, CancellationToken cancellationToken = default);
    Task<List<(int QueueItemId, int UserProfileId)>> DequeueBatchAsync(int batchSize, CancellationToken cancellationToken = default);
    Task RemoveFromQueueAsync(int queueItemId, CancellationToken cancellationToken = default);
    Task RequeueWithRetryAsync(int queueItemId, CancellationToken cancellationToken = default);
    Task<int> GetQueueCountAsync(CancellationToken cancellationToken = default);
    Task ClearQueueAsync(CancellationToken cancellationToken = default);
}

