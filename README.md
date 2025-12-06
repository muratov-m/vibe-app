# Vibe App - ASP.NET Core Web Application

Минимальный ASP.NET Core веб-сайт с авторизацией и профилем пользователя для конкурса Vibe Coding.

## Функционал

- ✅ Регистрация и вход пользователей
- ✅ Профиль пользователя с возможностью изменения пароля
- ✅ ASP.NET Core Identity для авторизации
- ✅ **UserProfile API** - управление расширенными профилями пользователей
- ✅ **User Profile Embeddings** - автоматическая генерация векторных представлений профилей (pgvector)
- ✅ **Embedding Queue** - асинхронная очередь для генерации embeddings через OpenAI
- ✅ **RAG Search** - семантический поиск профилей пользователей
- ✅ **OpenAI Gateway** - интеграция с OpenAI Chat Completions API
- ✅ **Vue 3 Frontend** - современный одностраничный интерфейс для RAG поиска
- ✅ **Простая архитектура** - все через constructor injection, без Service Locator
- ✅ **Postman коллекция** для тестирования API
- ✅ PostgreSQL для хранения данных
- ✅ Современный UI на Bootstrap 5
- ✅ Web API с Swagger
- ✅ Docker поддержка

## Технологии

### Backend
- ASP.NET Core 9.0
- ASP.NET Core Identity
- Razor Pages
- PostgreSQL (через Npgsql.EntityFrameworkCore.PostgreSQL)
- **pgvector** - векторные расширения для PostgreSQL
- **OpenAI** - официальная библиотека OpenAI для .NET
- Bootstrap 5
- Docker
- Swagger/OpenAPI

### Frontend
- **Vue 3** - Composition API
- **Vite** - Build tool
- **Tailwind CSS** - Utility-first CSS framework

## Локальная разработка

### Требования

- .NET 9.0 SDK
- PostgreSQL (или Docker для запуска PostgreSQL)
- **pgvector extension** для PostgreSQL (для работы с embeddings)
- Node.js 18+ и npm (для фронтенда)

### Настройка PostgreSQL локально

#### Вариант 1: Docker
```bash
# PostgreSQL с pgvector extension
docker run --name postgres-vibe \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=vibeapp \
  -p 5432:5432 \
  -d pgvector/pgvector:pg16
```

#### Вариант 2: Установленный PostgreSQL
Создайте базу данных `vibeapp` с пользователем `postgres` и паролем `postgres`.

**Установка pgvector extension:**
- **Windows/macOS**: Скачайте и установите из [pgvector releases](https://github.com/pgvector/pgvector/releases)
- **Linux**: `sudo apt install postgresql-16-pgvector` (для PostgreSQL 16)
- **Render.com**: pgvector доступен по умолчанию

После установки выполните в базе данных:
```sql
CREATE EXTENSION IF NOT EXISTS vector;
```

### Запуск приложения

```bash
cd src/VibeApp.Api
dotnet restore
dotnet ef database update  # Применить миграции Identity
dotnet run
```

Или из корня с использованием solution:

```bash
dotnet restore
dotnet build
cd src/VibeApp.Api
dotnet run
```

Приложение будет доступно по адресу:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: http://localhost:5000/swagger

### Запуск Frontend (Vue 3 + Vite)

**Вариант 1: Development режим (с hot reload)**

Backend (Terminal 1):
```bash
cd src/VibeApp.Api
dotnet run
```

Frontend (Terminal 2):
```bash
cd src\frontend
npm install
npm run dev
```

Frontend будет доступен по адресу: http://localhost:5173

**Вариант 2: Production режим (встроенный в backend)**

Соберите фронтенд и запустите только backend:

Windows:
```bash
build.cmd
cd src\VibeApp.Api
dotnet run
```

Linux/macOS:
```bash
chmod +x build.sh
./build.sh
cd src/VibeApp.Api
dotnet run
```

Vue приложение будет встроено в backend: http://localhost:5000

**Подробнее:** `docs/FRONTEND_INTEGRATION.md`

### Страницы сайта

**Backend (Razor Pages):**
- `/` - Главная страница (или Vue SPA если встроен)
- `/Account/Register` - Регистрация
- `/Account/Login` - Вход
- `/Account/Profile` - Профиль пользователя (требует авторизации)
- `/Account/Logout` - Выход

**Frontend (Vue SPA):**
- http://localhost:5173 - RAG Search интерфейс (dev mode)
- http://localhost:5000 - RAG Search интерфейс (production, встроен в backend)

**Подробнее:** `docs/FRONTEND_INTEGRATION.md`

### Создание и применение миграций

```bash
cd src/VibeApp.Api

# Создать миграцию
dotnet ef migrations add MigrationName

# Применить миграцию
dotnet ef database update
```

## Деплой на Render.com

```bash
git add .
git commit -m "Deploy with Vue frontend"
git push
```

Render автоматически соберёт Vue frontend и задеплоит приложение (~3-4 минуты).

**После деплоя:**
- `https://your-app.onrender.com/` → Vue SPA ✅
- `https://your-app.onrender.com/api/*` → API

**Environment Variable:** Добавьте `OPENAI_API_KEY` в Render Dashboard

**Подробнее:** `docs/RENDER_DEPLOYMENT.md`

## API Endpoints

### Authentication & Pages
- `/` - Главная страница
- `/Account/Register` - Регистрация
- `/Account/Login` - Вход
- `/Account/Profile` - Профиль пользователя (требует авторизации)
- `/Account/Logout` - Выход

### User Profile API
- `POST /api/userprofile/import` - Импорт профиля пользователя из JSON
- `GET /api/userprofile` - Получить все профили
- `GET /api/userprofile/{id}` - Получить профиль по ID
- `PUT /api/userprofile/{id}` - Обновить профиль
- `DELETE /api/userprofile/{id}` - Удалить профиль

### RAG Search API
- `POST /api/ragsearch/search` - Семантический поиск профилей через RAG
- `GET /api/ragsearch/search?q={query}` - Быстрый поиск через GET

### Embedding Queue API
- `GET /api/embedding-queue/status` - Статус очереди генерации embeddings
- `POST /api/embedding-queue/clear` - Очистить очередь

### Utility
- `GET /api/weatherforecast` - Тестовый endpoint с прогнозом погоды
- `GET /health` - Health check endpoint

### Пример импорта профиля

```bash
curl -X POST http://localhost:5000/api/userprofile/import \
  -H "Content-Type: application/json" \
  -d @sample-user-profile.json
```

Или используйте Swagger UI: http://localhost:5000/swagger

## OpenAI Gateway

Приложение включает Gateway для работы с OpenAI Chat API через официальную библиотеку OpenAI для .NET.

### Настройка

Установите environment variable `OPENAI_API_KEY`:

**Локально (Windows PowerShell):**
```powershell
$env:OPENAI_API_KEY="sk-your-api-key-here"
```

**Локально (Linux/macOS):**
```bash
export OPENAI_API_KEY="sk-your-api-key-here"
```

**Render.com:**
Добавьте environment variable `OPENAI_API_KEY` в настройках Web Service

### Использование в коде

```csharp
public class MyService
{
    private readonly IOpenAIGateway _openAIGateway;

    public MyService(IOpenAIGateway openAIGateway)
    {
        _openAIGateway = openAIGateway;
    }

    public async Task<string> GetChatResponse()
    {
        var messages = new[]
        {
            ChatMessage.CreateSystemMessage("You are a helpful assistant."),
            ChatMessage.CreateUserMessage("What is the capital of France?")
        };

        // Обычный ответ
        var response = await _openAIGateway.CreateChatCompletionAsync(
            messages,
            model: "gpt-4o-mini",
            temperature: 0.7f
        );

        return response;
    }

    public async IAsyncEnumerable<string> GetStreamingResponse()
    {
        var messages = new[]
        {
            ChatMessage.CreateSystemMessage("You are a helpful assistant."),
            ChatMessage.CreateUserMessage("Tell me a story.")
        };

        // Streaming ответ
        await foreach (var chunk in _openAIGateway.CreateChatCompletionStreamAsync(messages))
        {
            yield return chunk;
        }
    }
}
```

### Параметры

- `model` - модель OpenAI (по умолчанию: `gpt-4o-mini`)
- `temperature` - "творческость" ответа, 0.0-2.0 (по умолчанию: 0.7)
- `maxTokens` - максимальное количество токенов в ответе (опционально)

## Тестирование API

### Вариант 1: Postman (Рекомендуется)

1. Импортируйте коллекцию из `docs/VibeApp-UserProfile-API.postman_collection.json`
2. Убедитесь, что переменная `base_url` = `http://localhost:5000`
3. Запустите запросы из коллекции

Подробная инструкция: `docs/POSTMAN_GUIDE.md`

### Автоматическая генерация Embeddings

При создании или обновлении профиля пользователя автоматически генерируется векторное представление (embedding):

1. **Асинхронная очередь**: Профили добавляются в очередь для обработки
2. **Фоновый сервис**: `EmbeddingProcessingService` обрабатывает очередь каждые 5 секунд
3. **OpenAI Integration**: Использует `text-embedding-3-small` модель для генерации embeddings
4. **Автоматическое удаление**: При удалении профиля embedding также удаляется
5. **pgvector таблица**: Embeddings хранятся в таблице `UserProfileEmbeddings` с типом `vector(1536)`
6. **Мониторинг**: API endpoints для просмотра статуса очереди и управления

Подробнее: `docs/EMBEDDING_QUEUE_GUIDE.md`

### Вариант 2: Swagger UI

Откройте http://localhost:5000/swagger и тестируйте через веб-интерфейс.

### Вариант 3: PowerShell/cURL

```powershell
# Импорт профилей
$json = Get-Content -Raw sample-user-profiles-batch.json
Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile/import" `
  -Method Post -Body $json -ContentType "application/json"
```

## Архитектура

Проект использует многослойную архитектуру (Layered Architecture):

### 1. **VibeApp.Api** - Презентационный слой
- Razor Pages для UI
- API контроллеры для REST endpoints
- Конфигурация middleware и DI
- Зависит от: VibeApp.Core, VibeApp.Data

### 2. **VibeApp.Core** - Слой бизнес-логики
- Интерфейсы сервисов (`IUserService`, `IUserProfileService`, `IUserProfileEmbeddingService`, `IRepository<T>`)
- Реализация бизнес-логики (`UserService`, `UserProfileService`, `UserProfileEmbeddingService`)
- Доменные модели (entities)
- Все зависимости через constructor injection
- Независим от конкретных технологий (EF Core, ASP.NET)
- Зависит от: Microsoft.AspNetCore.Identity.EntityFrameworkCore, Pgvector

### 3. **VibeApp.Data** - Слой доступа к данным
- Entity Framework Core DbContext
- Репозитории для работы с данными
- Миграции базы данных
- Зависит от: VibeApp.Core, PostgreSQL

### Принципы
- **Разделение ответственности**: каждый слой имеет свою задачу
- **Dependency Inversion**: зависимости через интерфейсы
- **Testability**: бизнес-логика изолирована и легко тестируется


## Разработка

### Добавление новых endpoints

1. Создайте новый контроллер в `src/VibeApp.Api/Controllers/`
2. Наследуйте от `ControllerBase` и добавьте атрибуты `[ApiController]` и `[Route]`
3. Реализуйте необходимые HTTP методы

### Работа с базой данных

1. Добавьте модели в `src/VibeApp.Core/Entities/`
2. Обновите `AppDbContext` в `src/VibeApp.Data/` для включения новых `DbSet<>`
3. Создайте и примените миграцию:

```bash
cd src/VibeApp.Api
dotnet ef migrations add MigrationName --project ../VibeApp.Data
dotnet ef database update
```

### База данных Identity

Приложение использует стандартные таблицы ASP.NET Core Identity:
- `AspNetUsers` - Пользователи
- `AspNetRoles` - Роли
- `AspNetUserRoles` - Связь пользователей и ролей
- `AspNetUserClaims` - Claims пользователей
- `AspNetUserLogins` - Внешние логины
- `AspNetUserTokens` - Токены пользователей
- `AspNetRoleClaims` - Claims ролей

## Настройки безопасности

Настройки паролей в `Program.cs`:
- Минимальная длина: 6 символов
- Требуется цифра: Да
- Требуется строчная буква: Да
- Требуется заглавная буква: Да
- Требуется специальный символ: Нет
- Уникальный email: Да

## Troubleshooting

### Ошибка подключения к PostgreSQL

Убедитесь, что:
- PostgreSQL запущен
- Параметры подключения в `appsettings.json` корректны
- Firewall не блокирует порт 5432

### Проблемы с деплоем на Render

- Проверьте логи в Render Dashboard
- Убедитесь, что переменная `DATABASE_URL` правильно настроена
- Проверьте, что health check endpoint `/health` доступен

### Ошибки миграций

Если миграции не применяются:
```bash
cd src/VibeApp.Api
dotnet ef database drop  # Удалить базу (ОСТОРОЖНО!)
dotnet ef database update  # Применить миграции заново
```

## Лицензия

MIT


