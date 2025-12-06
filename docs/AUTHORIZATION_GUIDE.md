# Authorization Guide

## Обзор
VibeApp использует ASP.NET Core Identity для управления пользователями и ролями.

## Создание первого Admin пользователя

### Вариант 1: Через веб-интерфейс + Миграция (Рекомендуется)

1. **Зарегистрируйте пользователя:**
   - Откройте http://localhost:5000/Account/Register
   - Email: `admin@vibe-app.com`
   - Password: `admin` (или любой пароль минимум 3 символа)

2. **Примените миграцию:**
```bash
cd src/VibeApp.Api
dotnet ef database update
```

Миграция `AddAdminRole` автоматически:
- ✅ Создаст роль "Admin" (если не существует)
- ✅ Найдет пользователя с email `admin@vibe-app.com`
- ✅ Назначит роль Admin

3. **Войдите:**
   - Откройте http://localhost:5000/Account/Login
   - Войдите с этими же credentials

### Вариант 2: Через API

```bash
# 1. Зарегистрируйте пользователя
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@vibe-app.com","password":"admin"}'

# 2. Примените миграцию
cd src/VibeApp.Api
dotnet ef database update

# 3. Войдите
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -c cookies.txt \
  -d '{"email":"admin@vibe-app.com","password":"admin"}'
```

### Вариант 3: Development only - init-admin endpoint

Только в Development окружении:
```bash
curl -X POST http://localhost:5000/api/auth/init-admin \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@vibe-app.com","password":"admin"}'
```

Этот endpoint создаст пользователя И назначит роль Admin за один запрос.

## Роли

### Admin
Роль администратора имеет доступ к следующим операциям:
- **EmbeddingQueueController** (все endpoints):
  - `GET /api/embedding-queue/status` - Статус очереди эмбеддингов
  - `POST /api/embedding-queue/clear` - Очистка очереди

- **UserProfileController** (административные операции):
  - `POST /api/userprofile/import` - Импорт профилей пользователей
  - `PUT /api/userprofile/{id}` - Обновление профиля
  - `DELETE /api/userprofile/{id}` - Удаление профиля

- **AuthController**:
  - `POST /api/auth/assign-role` - Назначение роли
  - `POST /api/auth/remove-role` - Удаление роли

### Публичные операции
Доступны без авторизации:
- `GET /api/userprofile` - Получить все профили
- `GET /api/userprofile/{id}` - Получить профиль по ID
- `POST /api/ragsearch/search` - RAG поиск
- `GET /api/country` - Получить страны

## API Endpoints

### 1. Регистрация пользователя
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123"
}
```

**Response:**
```json
{
  "message": "User registered successfully",
  "userId": "user-id-guid"
}
```

### 2. Вход (Login)
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123",
  "rememberMe": false
}
```

**Response:**
```json
{
  "message": "Login successful",
  "userId": "user-id-guid",
  "email": "user@example.com",
  "roles": ["Admin"]
}
```

**Note:** Использует cookie-based authentication. Cookies автоматически устанавливаются браузером.

### 3. Выход (Logout)
```http
POST /api/auth/logout
Authorization: Required (cookie-based)
```

**Response:**
```json
{
  "message": "Logout successful"
}
```

### 4. Получить информацию о текущем пользователе
```http
GET /api/auth/me
Authorization: Required (cookie-based)
```

**Response (успешно):**
```json
{
  "userId": "user-id-guid",
  "email": "user@example.com",
  "roles": ["Admin"]
}
```

**Response (не авторизован - 401):**
```json
{
  "error": "User is not authenticated"
}
```

### 5. Назначить роль пользователю (Admin only)
```http
POST /api/auth/assign-role
Authorization: Required (Admin role)
Content-Type: application/json

{
  "email": "user@example.com",
  "role": "Admin"
}
```

**Response:**
```json
{
  "message": "Role 'Admin' assigned successfully"
}
```

### 6. Удалить роль у пользователя (Admin only)
```http
POST /api/auth/remove-role
Authorization: Required (Admin role)
Content-Type: application/json

{
  "email": "user@example.com",
  "role": "Admin"
}
```

**Response:**
```json
{
  "message": "Role 'Admin' removed successfully"
}
```

### 7. Инициализация Admin пользователя (Development only)
```http
POST /api/auth/init-admin
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "Admin123"
}
```

**Response:**
```json
{
  "message": "Admin user created successfully",
  "userId": "user-id-guid"
}
```

**Note:** Этот endpoint доступен только в Development окружении.

## Использование в Postman

### Импорт коллекции

1. Импортируйте коллекцию: `docs/VibeApp-Auth-API.postman_collection.json`

2. Переменные окружения уже настроены в коллекции:
   - `baseUrl`: `http://localhost:5000`
   - `adminEmail`: `admin@vibeapp.com`
   - `adminPassword`: `Admin123`
   - `userEmail`: `user@vibeapp.com`
   - `userPassword`: `User123`

### Быстрый тест авторизации

Запустите запросы в следующем порядке:

1. **Initialize Admin** - Создаст первого admin пользователя
2. **Login as Admin** - Войдет под admin (cookies сохранятся автоматически)
3. **Get Current User Info** - Проверит что вы залогинены
4. **Get Embedding Queue Status** - Тест доступа к admin endpoint
5. **Logout** - Выйдет из системы
6. **Test Unauthorized Access** - Проверит что без авторизации endpoint недоступен

### Настройка cookie-based authentication

1. **Создайте Admin пользователя** (только в development):
   ```
   POST http://localhost:5000/api/auth/init-admin
   {
     "email": "admin@example.com",
     "password": "Admin123"
   }
   ```

2. **Войдите в систему**:
   ```
   POST http://localhost:5000/api/auth/login
   {
     "email": "admin@example.com",
     "password": "Admin123"
   }
   ```
   
   Postman автоматически сохранит cookies из ответа.

3. **Используйте защищенные endpoints**:
   - Cookies будут автоматически отправляться с каждым запросом
   - Убедитесь, что в Postman включена настройка "Automatically follow redirects"
   - В Settings → General → "Cookie jar" должен быть включен

### Проверка авторизации

Попробуйте вызвать защищенный endpoint без входа:
```
GET http://localhost:5000/api/embedding-queue/status
```

**Expected Response (без авторизации):**
```
Status: 401 Unauthorized
```

**После login:**
```
Status: 200 OK
{
  "profilesInQueue": 0,
  "timestamp": "2024-12-06T..."
}
```

## Использование в Swagger UI

1. Откройте Swagger UI: `http://localhost:5000/swagger`

2. Нажмите кнопку **"Authorize"** вверху справа

3. В поле "Value" введите: `Bearer your-token-here`
   
   **Note:** В текущей реализации используется cookie-based authentication, поэтому для Swagger нужно сначала залогиниться через `/api/auth/login` endpoint

4. После успешного логина можете тестировать защищенные endpoints

## Обработка ошибок

### 401 Unauthorized
Пользователь не авторизован. Необходимо выполнить login.

```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### 403 Forbidden
Пользователь авторизован, но не имеет необходимой роли.

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403
}
```

## Production Deployment

### На Render.com

1. **Создайте первого Admin пользователя** через API или базу данных напрямую

2. **Настройте переменные окружения** (если потребуется):
   - `ASPNETCORE_ENVIRONMENT=Production`

3. **Cookie Security**: В production автоматически включается:
   - `Secure` cookies (только HTTPS)
   - `SameSite=Strict`
   - `HttpOnly=true`

## Безопасность

### Требования к паролю (упрощенные для demo/hackathon)
- Минимум 3 символа
- Не требуются цифры, заглавные буквы или специальные символы
- Email должен быть уникальным

**Примеры валидных паролей:** `123`, `abc`, `admin`, `test`

**Примечание:** Для production рекомендуется усилить требования в `Program.cs`:
```csharp
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequiredLength = 8;
```

### Cookie Settings
- `HttpOnly`: Защита от XSS атак
- `Secure`: Только HTTPS в production
- `SameSite`: Защита от CSRF атак
- Login path: `/Account/Login`
- Logout path: `/Account/Logout`

## Расширение системы ролей

Для добавления новых ролей:

1. Создайте роль программно:
```csharp
if (!await _roleManager.RoleExistsAsync("Manager"))
{
    await _roleManager.CreateAsync(new IdentityRole("Manager"));
}
```

2. Используйте атрибут `[Authorize]`:
```csharp
[HttpGet("manager-only")]
[Authorize(Roles = "Manager")]
public async Task<IActionResult> ManagerEndpoint()
{
    // ...
}
```

3. Или несколько ролей:
```csharp
[Authorize(Roles = "Admin,Manager")]
public async Task<IActionResult> AdminOrManagerEndpoint()
{
    // ...
}
```

