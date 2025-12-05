# UserProfile API Testing Guide

## Quick Start

Application is running at: http://localhost:5000

**Swagger UI:** http://localhost:5000/swagger

## Sample Data Import

Use the provided `sample-user-profile.json` file to test the API.

### PowerShell (Windows)
```powershell
$json = Get-Content -Raw sample-user-profile.json
Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile/import" -Method Post -Body $json -ContentType "application/json"
```

### Bash/cURL
```bash
curl -X POST http://localhost:5000/api/userprofile/import \
  -H "Content-Type: application/json" \
  -d @sample-user-profile.json
```

## API Endpoints

### 1. Import User Profile
**POST** `/api/userprofile/import`

Import a user profile from JSON format.

**Request Body:**
```json
{
  "id": 1,
  "name": "Elena Saf",
  "telegram": "dmitry_dev",
  "linkedin": "https://www.linkedin.com/in/esaf20",
  "bio": "QA Lead with department management experience",
  "skills": ["коучинг", "QA менторинг", "тестирование"],
  "hasStartup": false,
  "startupStage": "",
  "startupDescription": "",
  "startupName": "",
  "lookingFor": ["Единомышленников"],
  "canHelp": "Help text here...",
  "needsHelp": "Looking for...",
  "aiUsage": "",
  "email": "",
  "photo": "",
  "custom_1": "",
  "custom_array_1": []
}
```

**Response:**
```json
{
  "id": 1,
  "message": "User profile imported successfully"
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

1. **Import a profile:**
   ```powershell
   $json = Get-Content -Raw sample-user-profile.json
   $result = Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile/import" -Method Post -Body $json -ContentType "application/json"
   $profileId = $result.id
   ```

2. **Get the profile:**
   ```powershell
   Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile/$profileId"
   ```

3. **Update the profile:**
   ```powershell
   $json = Get-Content -Raw sample-user-profile.json
   # Modify the JSON as needed
   Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile/$profileId" -Method Put -Body $json -ContentType "application/json"
   ```

4. **Delete the profile:**
   ```powershell
   Invoke-RestMethod -Uri "http://localhost:5000/api/userprofile/$profileId" -Method Delete
   ```

## Architecture

The implementation follows a layered architecture:

- **VibeApp.Api** - Controllers and API endpoints
- **VibeApp.Core** - Business logic (UserProfileService, interfaces, DTOs)
- **VibeApp.Data** - Data access (EF Core, repositories, migrations)

All operations are asynchronous and include proper error handling.

