# User Matching Embeddings

## Overview
UserMatchingEmbedding - отдельная система эмбеддингов для поиска совпадений между пользователями (matching). В отличие от UserProfileEmbedding, которое использует весь профиль для общего семантического поиска, UserMatchingEmbedding фокусируется только на ключевых параметрах для подбора совпадений.

## Отличия от UserProfileEmbedding

| Параметр | UserProfileEmbedding | UserMatchingEmbedding |
|----------|---------------------|---------------------|
| **Цель** | Общий семантический поиск по профилям | Поиск совпадений (matching) между пользователями |
| **Используемые поля** | Bio, Skills, LookingFor, Startup info, Can Help, Needs Help, AI Usage, Parsed fields | Только: ParsedInterests, ParsedMainActivity, ParsedCountry, ParsedCity |
| **Таблица** | `UserProfileEmbeddings` | `UserMatchingEmbeddings` |
| **Сервис** | `UserProfileEmbeddingService` | `UserMatchingEmbeddingService` |

## Архитектура

### Сущность
```csharp
public class UserMatchingEmbedding
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public Vector Embedding { get; set; } // 1536 dimensions
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public UserProfile UserProfile { get; set; }
}
```

### Формирование эмбеддинга
Строка для эмбеддинга строится только из parsed полей:
```
Interests: {ParsedInterests}
Main activity: {ParsedMainActivity}
Country: {ParsedCountry}
City: {ParsedCity}
```

### Автоматическая обработка
При обработке профиля через `EmbeddingProcessingService` выполняется **3 шага**:
1. **Парсинг профиля** через `UserProfileParsingService` - заполняет Parsed* поля
2. **UserProfileEmbedding** - для общего поиска (использует весь профиль включая parsed поля)
3. **UserMatchingEmbedding** - для matching (использует только parsed поля из шага 1)

## API Endpoints

### Автоматическая генерация через очередь
Matching embeddings создаются автоматически при добавлении профиля в очередь:
```http
POST /api/user-profile/{id}/queue-embedding
```

Фоновый сервис автоматически:
1. Парсит профиль через AI
2. Генерирует UserProfileEmbedding
3. Генерирует UserMatchingEmbedding

**Примечание**: Отдельных API endpoints для управления matching embeddings нет - они создаются и обновляются автоматически вместе с profile embeddings через фоновый сервис.

## Использование

### Через очередь (рекомендуемый способ)
Когда профиль добавляется в очередь через:
```http
POST /api/user-profile/{id}/queue-embedding
```

Background service автоматически выполняет полный цикл обработки:
1. Парсит профиль через OpenAI (заполняет ParsedShortBio, ParsedMainActivity, ParsedInterests, ParsedCountry, ParsedCity)
2. Создает UserProfileEmbedding из всего профиля
3. Создает UserMatchingEmbedding из parsed полей

## Database Schema

### Таблица UserMatchingEmbeddings
```sql
CREATE TABLE "UserMatchingEmbeddings" (
    "Id" SERIAL PRIMARY KEY,
    "UserProfileId" INTEGER NOT NULL UNIQUE,
    "Embedding" vector(1536) NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY ("UserProfileId") REFERENCES "UserProfiles"("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_UserMatchingEmbeddings_UserProfileId" 
ON "UserMatchingEmbeddings" ("UserProfileId");
```

## Примеры использования

### Поиск похожих пользователей для matching
```csharp
// В будущем можно создать RagMatchingSearchService
// который будет искать похожих пользователей используя UserMatchingEmbedding
// вместо UserProfileEmbedding для более точного matching
```

### Различия в поиске
- **RAG Search с UserProfileEmbedding**: "Найди всех разработчиков с опытом в AI"
  - Использует полный профиль, включая Bio, Skills, Startup description
  
- **Matching с UserMatchingEmbedding**: "Найди пользователей с похожими интересами и локацией"
  - Использует только структурированные данные: интересы, активность, страна, город
  - Более точный для подбора пар/групп

## Migration
Миграция создана: `20251206130015_AddUserMatchingEmbedding.cs`

Применение:
```bash
dotnet ef database update --project src/VibeApp.Data --startup-project src/VibeApp.Api
```

