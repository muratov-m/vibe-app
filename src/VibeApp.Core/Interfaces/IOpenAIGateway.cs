using OpenAI.Chat;

namespace VibeApp.Core.Interfaces;

/// <summary>
/// Gateway for interacting with OpenAI Chat API
/// </summary>
public interface IOpenAIGateway
{
    /// <summary>
    /// Create a chat completion
    /// </summary>
    /// <param name="messages">List of messages in the conversation</param>
    /// <param name="model">Model to use (default: gpt-4o-mini)</param>
    /// <param name="temperature">Sampling temperature (default: 0.7)</param>
    /// <param name="maxTokens">Maximum tokens in response (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Assistant's response message</returns>
    Task<string> CreateChatCompletionAsync(
        IEnumerable<ChatMessage> messages,
        string model = "gpt-4o-mini",
        float temperature = 0.7f,
        int? maxTokens = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a chat completion with streaming response
    /// </summary>
    /// <param name="messages">List of messages in the conversation</param>
    /// <param name="model">Model to use (default: gpt-4o-mini)</param>
    /// <param name="temperature">Sampling temperature (default: 0.7)</param>
    /// <param name="maxTokens">Maximum tokens in response (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async enumerable of response chunks</returns>
    IAsyncEnumerable<string> CreateChatCompletionStreamAsync(
        IEnumerable<ChatMessage> messages,
        string model = "gpt-4o-mini",
        float temperature = 0.7f,
        int? maxTokens = null,
        CancellationToken cancellationToken = default);
}

