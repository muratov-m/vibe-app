using VibeApp.Core.DTOs;

namespace VibeApp.Core.Interfaces;

/// <summary>
/// Service for RAG (Retrieval Augmented Generation) search across user profiles
/// </summary>
public interface IRagSearchService
{
    /// <summary>
    /// Search for user profiles matching a natural language query
    /// </summary>
    /// <param name="request">Search request containing the query and options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results with matching profiles and optional LLM-generated response</returns>
    Task<RagSearchResponseDto> SearchAsync(RagSearchRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate embedding for a query text
    /// </summary>
    /// <param name="query">Query text</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Embedding vector as float array</returns>
    Task<float[]> GenerateQueryEmbeddingAsync(string query, CancellationToken cancellationToken = default);
}

