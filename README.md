# Vibe App - ASP.NET Core Web API

Минимальный ASP.NET Core Web API проект для конкурса Vibe Coding с поддержкой PostgreSQL.

## Технологии

- ASP.NET Core 9.0
- PostgreSQL (через Npgsql.EntityFrameworkCore.PostgreSQL)
- Docker
- Swagger/OpenAPI

## Локальная разработка

### Требования

- .NET 9.0 SDK
- PostgreSQL (или Docker для запуска PostgreSQL)

### Настройка PostgreSQL локально

#### Вариант 1: Docker
```bash
docker run --name postgres-vibe -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=vibeapp -p 5432:5432 -d postgres:16
```

#### Вариант 2: Установленный PostgreSQL
Создайте базу данных `vibeapp` с пользователем `postgres` и паролем `postgres`.

### Запуск приложения

```bash
cd src/VibeApp.Api
dotnet restore
dotnet run
```

Приложение будет доступно по адресу:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: http://localhost:5000/swagger

### Создание и применение миграций

```bash
cd src/VibeApp.Api

# Создать миграцию
dotnet ef migrations add InitialCreate

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

- `GET /api/weatherforecast` - Тестовый endpoint с прогнозом погоды
- `GET /health` - Health check endpoint

## Структура проекта

```
vibe-app/
├── src/
│   └── VibeApp.Api/          # Основной API проект
│       ├── Controllers/       # API контроллеры
│       ├── Data/             # DbContext и модели данных
│       ├── Properties/       # Настройки запуска
│       ├── appsettings.json  # Конфигурация
│       └── Program.cs        # Точка входа приложения
├── docs/
│   └── VIBE_LOG.md          # Лог разработки
├── Dockerfile               # Docker конфигурация
├── render.yaml              # Конфигурация для Render.com
└── README.md               # Этот файл
```

## Разработка

### Добавление новых endpoints

1. Создайте новый контроллер в `src/VibeApp.Api/Controllers/`
2. Наследуйте от `ControllerBase` и добавьте атрибуты `[ApiController]` и `[Route]`
3. Реализуйте необходимые HTTP методы

### Работа с базой данных

1. Добавьте модели в `src/VibeApp.Api/Data/`
2. Обновите `AppDbContext` для включения новых `DbSet<>`
3. Создайте и примените миграцию

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

## Лицензия

MIT

