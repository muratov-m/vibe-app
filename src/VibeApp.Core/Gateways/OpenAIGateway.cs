using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using VibeApp.Core.Interfaces;

namespace VibeApp.Core.Gateways;

public class OpenAIGateway : IOpenAIGateway
{
    private readonly string _apiKey;
    private readonly ILogger<OpenAIGateway> _logger;

    public OpenAIGateway(ILogger<OpenAIGateway> logger)
    {
        _logger = logger;

        _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
            ?? throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set");

        _logger.LogInformation("OpenAI Gateway initialized successfully");
    }

    public async Task<string> CreateChatCompletionAsync(
        IEnumerable<ChatMessage> messages,
        string model = "gpt-4o-mini",
        float temperature = 0.7f,
        int? maxTokens = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating chat completion with model {Model}", model);

            var options = new ChatCompletionOptions
            {
                Temperature = temperature,
                MaxOutputTokenCount = maxTokens
            };

            var openAiClient = new OpenAIClient(_apiKey);
            var chatClient = openAiClient.GetChatClient(model);
            var completion = await chatClient.CompleteChatAsync(messages, options, cancellationToken);
            var responseContent = completion.Value.Content[0].Text;

            _logger.LogInformation("Chat completion created successfully. Response length: {Length} characters", 
                responseContent.Length);

            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating chat completion with model {Model}", model);
            throw;
        }
    }

    public async IAsyncEnumerable<string> CreateChatCompletionStreamAsync(
        IEnumerable<ChatMessage> messages,
        string model = "gpt-4o-mini",
        float temperature = 0.7f,
        int? maxTokens = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating streaming chat completion with model {Model}", model);

        var options = new ChatCompletionOptions
        {
            Temperature = temperature,
            MaxOutputTokenCount = maxTokens
        };

        var openAiClient = new OpenAIClient(_apiKey);
        var chatClient = openAiClient.GetChatClient(model);

        await foreach (var update in chatClient.CompleteChatStreamingAsync(messages, options, cancellationToken))
        {
            foreach (var contentPart in update.ContentUpdate)
            {
                if (!string.IsNullOrEmpty(contentPart.Text))
                {
                    yield return contentPart.Text;
                }
            }
        }

        _logger.LogInformation("Streaming chat completion completed for model {Model}", model);
    }

    public async Task<float[]> GetEmbeddingAsync(
        string text,
        string model = "text-embedding-3-small",
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating embedding with model {Model}, text length: {Length}", model, text.Length);

            var openAiClient = new OpenAIClient(_apiKey);
            var embeddingClient = openAiClient.GetEmbeddingClient(model);
            var embedding = await embeddingClient.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);

            var vector = embedding.Value.ToFloats().ToArray();
            
            _logger.LogInformation("Embedding generated successfully. Dimension: {Dimension}", vector.Length);

            return vector;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding with model {Model}", model);
            throw;
        }
    }
}

