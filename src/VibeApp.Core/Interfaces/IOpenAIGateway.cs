using OpenAI.Chat;

namespace VibeApp.Core.Interfaces;

/// <summary>
/// Gateway for interacting with OpenAI API
/// </summary>
public interface IOpenAIGateway
{
    /// <summary>
    /// Create a chat completion
    /// </summary>
    Task<string> CreateChatCompletionAsync(
        IEnumerable<ChatMessage> messages,
        string model = "gpt-4o-mini",
        float temperature = 0.7f,
        int? maxTokens = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a chat completion with streaming response
    /// </summary>
    IAsyncEnumerable<string> CreateChatCompletionStreamAsync(
        IEnumerable<ChatMessage> messages,
        string model = "gpt-4o-mini",
        float temperature = 0.7f,
        int? maxTokens = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate embedding for text
    /// </summary>
    Task<float[]> GetEmbeddingAsync(
        string text,
        string model = "text-embedding-3-small",
        CancellationToken cancellationToken = default);
}

