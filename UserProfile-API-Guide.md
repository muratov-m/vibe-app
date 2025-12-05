# UserProfile API Testing Guide

## Quick Start

Application is running at: http://localhost:5000

**Swagger UI:** http://localhost:5000/swagger

## ⚠️ IMPORTANT: Batch Import with Sync

API endpoint `/api/userprofile/import` теперь работает с **массивом пользователей** и **синхронизирует** БД:

- ✅ Создаёт новых пользователей из списка
- ✅ Обновляет существующих пользователей
- ❌ **Удаляет пользователей, которых НЕТ в импортируемом списке**

**Это означает**: если в БД были пользователи с ID 1, 2, 3, а вы импортируете только [1, 4], то пользователи 2 и 3 будут удалены!

## Sample Data Import

Используйте файл `sample-user-profiles-batch.json` с массивом пользователей.

### PowerShell (Windows)
```powershell
$json = Get-Content -Raw sample-user-profiles-batch.json
Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile/import" -Method Post -Body $json -ContentType "application/json"
```

### Bash/cURL
```bash
curl -X POST http://localhost:5000/api/userprofile/import \
  -H "Content-Type: application/json" \
  -d @sample-user-profiles-batch.json
```

## API Endpoints

### 1. Import User Profiles (Batch with Sync)
**POST** `/api/userprofile/import`

Импортирует массив профилей и синхронизирует с БД (удаляет отсутствующих).

**Request Body (Array):**
```json
[
  {
    "id": 1,
    "name": "Elena Saf",
    "telegram": "dmitry_dev",
    "linkedin": "https://www.linkedin.com/in/esaf20",
    "bio": "QA Lead with department management experience",
    "skills": ["коучинг", "QA менторинг", "тестирование"],
    "hasStartup": false,
    "lookingFor": ["Единомышленников"],
    "canHelp": "Help text here...",
    "needsHelp": "Looking for...",
    ...
  },
  {
    "id": 2,
    "name": "Иван Петров",
    ...
  }
]
```

**Response:**
```json
{
  "totalProcessed": 2,
  "created": 1,
  "updated": 1,
  "deleted": 0,
  "errors": [],
  "message": "Successfully processed 2 profiles. Created: 1, Updated: 1, Deleted: 0"
}
```

### 2. Get All Profiles
**GET** `/api/userprofile`

Returns all user profiles with related data (skills, lookingFor, etc.)

### 3. Get Profile by ID
**GET** `/api/userprofile/{id}`

Returns a specific user profile by ID.

**Example:** `GET /api/userprofile/1`

### 4. Update Profile
**PUT** `/api/userprofile/{id}`

Updates an existing user profile. Same request body as import.

**Example:** `PUT /api/userprofile/1`

### 5. Delete Profile
**DELETE** `/api/userprofile/{id}`

Deletes a user profile and all related data.

**Example:** `DELETE /api/userprofile/1`

## Database Tables

The following tables are created in PostgreSQL:

- **UserProfiles** - Main profile data
  - Id, Name, Telegram, LinkedIn, Bio, Email, Photo
  - HasStartup, StartupStage, StartupDescription, StartupName
  - CanHelp, NeedsHelp, AiUsage
  - Custom1-7 (extensibility fields)
  - CreatedAt, UpdatedAt

- **UserSkills** - Skills array
- **UserLookingFors** - LookingFor array
- **UserCustomArray1s** through **UserCustomArray7s** - Custom arrays for extensibility

All relationships have cascade delete configured.

## Example Workflow

1. **First import (creates 2 users):**
   ```powershell
   $json = Get-Content -Raw sample-user-profiles-batch.json
   $result = Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile/import" -Method Post -Body $json -ContentType "application/json"
   Write-Host "Created: $($result.created), Updated: $($result.updated), Deleted: $($result.deleted)"
   ```
   Result: Created: 2, Updated: 0, Deleted: 0

2. **Second import with same data (updates 2 users):**
   ```powershell
   $result = Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile/import" -Method Post -Body $json -ContentType "application/json"
   Write-Host "Created: $($result.created), Updated: $($result.updated), Deleted: $($result.deleted)"
   ```
   Result: Created: 0, Updated: 2, Deleted: 0

3. **Third import with only user ID 1 (deletes user ID 2):**
   ```powershell
   $json = '[ { "id": 1, "name": "Elena", ... } ]'
   $result = Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile/import" -Method Post -Body $json -ContentType "application/json"
   Write-Host "Created: $($result.created), Updated: $($result.updated), Deleted: $($result.deleted)"
   ```
   Result: Created: 0, Updated: 1, Deleted: 1

4. **Get all profiles:**
   ```powershell
   Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile"
   ```

5. **Get specific profile:**
   ```powershell
   Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile/1"
   ```

## Architecture

The implementation follows a layered architecture:

- **VibeApp.Api** - Controllers and API endpoints
- **VibeApp.Core** - Business logic (UserProfileService, interfaces, DTOs)
- **VibeApp.Data** - Data access (EF Core, repositories, migrations)

All operations are asynchronous and include proper error handling.

