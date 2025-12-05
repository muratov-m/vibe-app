# OpenAI Gateway - Примеры использования

## Основная информация

OpenAI Gateway предоставляет простой интерфейс для работы с OpenAI Chat Completions API.

## Интерфейс

```csharp
public interface IOpenAIGateway
{
    // Обычный запрос с полным ответом
    Task<string> CreateChatCompletionAsync(
        IEnumerable<ChatMessage> messages,
        string model = "gpt-4o-mini",
        float temperature = 0.7f,
        int? maxTokens = null,
        CancellationToken cancellationToken = default);

    // Streaming запрос (ответ по частям)
    IAsyncEnumerable<string> CreateChatCompletionStreamAsync(
        IEnumerable<ChatMessage> messages,
        string model = "gpt-4o-mini",
        float temperature = 0.7f,
        int? maxTokens = null,
        CancellationToken cancellationToken = default);
}
```

## Пример 1: Простой вопрос-ответ

```csharp
public class ChatService
{
    private readonly IOpenAIGateway _openAIGateway;

    public ChatService(IOpenAIGateway openAIGateway)
    {
        _openAIGateway = openAIGateway;
    }

    public async Task<string> AskQuestionAsync(string question)
    {
        var messages = new[]
        {
            ChatMessage.CreateSystemMessage("You are a helpful assistant."),
            ChatMessage.CreateUserMessage(question)
        };

        var response = await _openAIGateway.CreateChatCompletionAsync(messages);
        return response;
    }
}
```

## Пример 2: Контекстный диалог

```csharp
public class ConversationService
{
    private readonly IOpenAIGateway _openAIGateway;
    private readonly List<ChatMessage> _conversationHistory = new();

    public ConversationService(IOpenAIGateway openAIGateway)
    {
        _openAIGateway = openAIGateway;
        
        // Инициализация с системным промптом
        _conversationHistory.Add(
            ChatMessage.CreateSystemMessage("You are a helpful coding assistant.")
        );
    }

    public async Task<string> SendMessageAsync(string userMessage)
    {
        // Добавляем сообщение пользователя в историю
        _conversationHistory.Add(ChatMessage.CreateUserMessage(userMessage));

        // Получаем ответ от AI
        var response = await _openAIGateway.CreateChatCompletionAsync(
            _conversationHistory,
            temperature: 0.7f
        );

        // Добавляем ответ AI в историю
        _conversationHistory.Add(ChatMessage.CreateAssistantMessage(response));

        return response;
    }

    public void ClearHistory()
    {
        _conversationHistory.Clear();
        _conversationHistory.Add(
            ChatMessage.CreateSystemMessage("You are a helpful coding assistant.")
        );
    }
}
```

## Пример 3: Streaming ответ

```csharp
public class StreamingChatService
{
    private readonly IOpenAIGateway _openAIGateway;

    public StreamingChatService(IOpenAIGateway openAIGateway)
    {
        _openAIGateway = openAIGateway;
    }

    public async IAsyncEnumerable<string> GetStreamingResponseAsync(string prompt)
    {
        var messages = new[]
        {
            ChatMessage.CreateSystemMessage("You are a creative storyteller."),
            ChatMessage.CreateUserMessage(prompt)
        };

        await foreach (var chunk in _openAIGateway.CreateChatCompletionStreamAsync(messages))
        {
            // Каждый chunk - это часть ответа
            yield return chunk;
        }
    }
}
```

## Пример 4: Анализ профиля пользователя

```csharp
public class UserProfileAnalyzer
{
    private readonly IOpenAIGateway _openAIGateway;

    public UserProfileAnalyzer(IOpenAIGateway openAIGateway)
    {
        _openAIGateway = openAIGateway;
    }

    public async Task<string> AnalyzeProfileAsync(UserProfile profile)
    {
        var profileText = $"""
            Name: {profile.Name}
            Bio: {profile.Bio}
            Skills: {string.Join(", ", profile.Skills.Select(s => s.Skill))}
            Looking for: {string.Join(", ", profile.LookingFor.Select(l => l.LookingFor))}
            """;

        var messages = new[]
        {
            ChatMessage.CreateSystemMessage(
                "You are an expert at analyzing user profiles and suggesting connections."
            ),
            ChatMessage.CreateUserMessage(
                $"Analyze this profile and suggest what kind of people or opportunities would be a good match:\n\n{profileText}"
            )
        };

        var response = await _openAIGateway.CreateChatCompletionAsync(
            messages,
            model: "gpt-4o-mini",
            temperature: 0.7f,
            maxTokens: 500
        );

        return response;
    }
}
```

## Пример 5: Использование в API Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IOpenAIGateway _openAIGateway;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IOpenAIGateway openAIGateway, ILogger<ChatController> logger)
    {
        _openAIGateway = openAIGateway;
        _logger = logger;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskQuestion([FromBody] ChatRequest request)
    {
        try
        {
            var messages = new[]
            {
                ChatMessage.CreateSystemMessage("You are a helpful assistant."),
                ChatMessage.CreateUserMessage(request.Question)
            };

            var response = await _openAIGateway.CreateChatCompletionAsync(messages);
            
            return Ok(new { answer = response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpPost("stream")]
    public async Task StreamResponse([FromBody] ChatRequest request)
    {
        Response.ContentType = "text/event-stream";

        var messages = new[]
        {
            ChatMessage.CreateSystemMessage("You are a helpful assistant."),
            ChatMessage.CreateUserMessage(request.Question)
        };

        await foreach (var chunk in _openAIGateway.CreateChatCompletionStreamAsync(messages))
        {
            await Response.WriteAsync($"data: {chunk}\n\n");
            await Response.Body.FlushAsync();
        }
    }
}

public class ChatRequest
{
    public string Question { get; set; } = string.Empty;
}
```

## Параметры

### model
Модели OpenAI:
- `gpt-4o-mini` (по умолчанию) - быстрая и дешевая модель
- `gpt-4o` - более мощная модель
- `gpt-4-turbo` - быстрая версия GPT-4
- `gpt-3.5-turbo` - старая модель (дешевле)

### temperature
Контролирует "творческость" ответов:
- `0.0` - детерминированный, фактический
- `0.7` - сбалансированный (по умолчанию)
- `1.0-2.0` - более творческий и случайный

### maxTokens
Максимальное количество токенов в ответе:
- `null` (по умолчанию) - без ограничений (используется лимит модели)
- `100-4000+` - ограничение длины ответа

## Обработка ошибок

```csharp
try
{
    var response = await _openAIGateway.CreateChatCompletionAsync(messages);
    return response;
}
catch (HttpRequestException ex)
{
    // Ошибка сети или API
    _logger.LogError(ex, "OpenAI API request failed");
    throw;
}
catch (InvalidOperationException ex)
{
    // API key не настроен
    _logger.LogError(ex, "OpenAI API key not configured");
    throw;
}
catch (Exception ex)
{
    // Другие ошибки
    _logger.LogError(ex, "Unexpected error calling OpenAI");
    throw;
}
```

## Конфигурация

### Environment Variable
API ключ читается из переменной окружения `OPENAI_API_KEY`.

**Локально (Windows PowerShell):**
```powershell
$env:OPENAI_API_KEY="sk-proj-xxx..."
```

**Локально (Linux/macOS):**
```bash
export OPENAI_API_KEY="sk-proj-xxx..."
```

**Render.com:**
Добавьте environment variable:
```
OPENAI_API_KEY=sk-proj-xxx...
```

