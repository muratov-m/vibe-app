# Vibe App - ASP.NET Core Web Application

Минимальный ASP.NET Core веб-сайт с авторизацией и профилем пользователя для конкурса Vibe Coding.

## Функционал

- ✅ Регистрация и вход пользователей
- ✅ Профиль пользователя с возможностью изменения пароля
- ✅ ASP.NET Core Identity для авторизации
- ✅ **UserProfile API** - управление расширенными профилями пользователей
- ✅ **User Profile Embeddings** - автоматическая генерация векторных представлений профилей (pgvector)
- ✅ **Простая архитектура** - все через constructor injection, без Service Locator
- ✅ **Postman коллекция** для тестирования API
- ✅ PostgreSQL для хранения данных
- ✅ Современный UI на Bootstrap 5
- ✅ Web API с Swagger
- ✅ Docker поддержка

## Технологии

- ASP.NET Core 9.0
- ASP.NET Core Identity
- Razor Pages
- PostgreSQL (через Npgsql.EntityFrameworkCore.PostgreSQL)
- **pgvector** - векторные расширения для PostgreSQL
- Bootstrap 5
- Docker
- Swagger/OpenAPI

## Локальная разработка

### Требования

- .NET 9.0 SDK
- PostgreSQL (или Docker для запуска PostgreSQL)
- **pgvector extension** для PostgreSQL (для работы с embeddings)

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

### Страницы сайта

- `/` - Главная страница
- `/Account/Register` - Регистрация
- `/Account/Login` - Вход
- `/Account/Profile` - Профиль пользователя (требует авторизации)
- `/Account/Logout` - Выход

### Создание и применение миграций

```bash
cd src/VibeApp.Api

# Создать миграцию
dotnet ef migrations add MigrationName

# Применить миграцию
dotnet ef database update
```

## Деплой на Render.com

### Автоматический деплой

1. Создайте аккаунт на [render.com](https://render.com)
2. Подключите ваш GitHub репозиторий
3. Render автоматически обнаружит файл `render.yaml` и создаст:
   - PostgreSQL базу данных
   - Web Service с Docker

### Ручная настройка

Если автоматический деплой не работает:

1. **Создайте PostgreSQL базу данных:**
   - Name: `vibe-app-db`
   - Database: `vibeapp`
   - User: `vibeapp`
   - Region: `Frankfurt` (или любой другой)

2. **Создайте Web Service:**
   - Environment: `Docker`
   - Build Command: (оставьте пустым, используется Dockerfile)
   - Start Command: (оставьте пустым, используется ENTRYPOINT из Dockerfile)
   - Environment Variables:
     - `ASPNETCORE_ENVIRONMENT`: `Production`
     - `ASPNETCORE_URLS`: `http://+:8080`
     - `DATABASE_URL`: подключите к созданной базе данных
   - Health Check Path: `/health`

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

## Тестирование API

### Вариант 1: Postman (Рекомендуется)

1. Импортируйте коллекцию из `docs/VibeApp-UserProfile-API.postman_collection.json`
2. Убедитесь, что переменная `base_url` = `http://localhost:5000`
3. Запустите запросы из коллекции

Подробная инструкция: `docs/POSTMAN_GUIDE.md`

### Автоматическая генерация Embeddings

При создании или обновлении профиля пользователя автоматически генерируется векторное представление (embedding):

1. **Синхронная обработка**: Embeddings генерируются в том же scope что и создание профиля
2. **Простая архитектура**: Все зависимости через constructor injection, без сложных фоновых сервисов
3. **Автоматическое удаление**: При удалении профиля embedding также удаляется
4. **pgvector таблица**: Embeddings хранятся в таблице `UserProfileEmbeddings` с типом `vector(1536)`

**Текущая реализация**: Генерация embedding является placeholder (возвращает нулевой вектор). В будущем здесь будет интеграция с OpenAI API или другим сервисом для генерации векторных представлений профиля.

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

