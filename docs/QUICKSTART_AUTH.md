# Быстрый старт - Система ролей

## Что реализовано

✅ Cookie-based authentication через ASP.NET Core Identity  
✅ Роль **Admin** для защиты административных endpoints  
✅ AuthController с полным набором операций  
✅ Защита EmbeddingQueueController (только Admin)  
✅ Защита административных операций UserProfileController  
✅ Swagger интеграция с отображением требований авторизации  
✅ Postman коллекция для тестирования  

## Защищенные endpoints

### Только Admin
- `GET /api/embedding-queue/status`
- `POST /api/embedding-queue/clear`
- `POST /api/userprofile/import`
- `PUT /api/userprofile/{id}`
- `DELETE /api/userprofile/{id}`
- `POST /api/auth/assign-role`
- `POST /api/auth/remove-role`

### Публичные (без авторизации)
- `GET /api/userprofile` - получить все профили
- `GET /api/userprofile/{id}` - получить профиль по ID
- `POST /api/ragsearch/search` - RAG поиск

## Быстрый тест

### Подготовка: Создание Admin пользователя

**Вариант 1: Через веб-интерфейс + Миграция (Рекомендуется)**

1. Зарегистрируйте пользователя с email `rnd.develop@gmail.com`:
   - Откройте http://localhost:5000/Account/Register
   - Email: `rnd.develop@gmail.com`
   - Password: `admin` (или любой пароль минимум 3 символа)

2. Примените миграцию (роль Admin назначится автоматически):
```bash
cd src/VibeApp.Api
dotnet ef database update
```

Миграция `AddAdminRole` автоматически:
- Создаст роль "Admin" если не существует
- Найдет пользователя с email `rnd.develop@gmail.com`
- Назначит роль Admin этому пользователю

**Вариант 2: Через API**

```bash
# 1. Зарегистрируйте пользователя
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"rnd.develop@gmail.com","password":"admin"}'

# 2. Примените миграцию
cd src/VibeApp.Api
dotnet ef database update
```

**Вариант 3: Через init-admin endpoint (Development only)**

**Вариант 3: Через init-admin endpoint (Development only)**

```bash
curl -X POST http://localhost:5000/api/auth/init-admin \
  -H "Content-Type: application/json" \
  -d '{"email":"rnd.develop@gmail.com","password":"admin"}'
```

### 1. Запустите приложение
```bash
cd src/VibeApp.Api
dotnet run
```

### 2. Убедитесь что пользователь rnd.develop@gmail.com существует
См. раздел "Подготовка: Создание Admin пользователя" выше

### 3. Войдите в систему
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -c cookies.txt \
  -d '{"email":"rnd.develop@gmail.com","password":"admin","rememberMe":false}'
```

**Ответ:**
```json
{
  "message": "Login successful",
  "userId": "guid-here",
  "email": "rnd.develop@gmail.com",
  "roles": ["Admin"]
}
```

**Важно:** Cookie автоматически сохранится в браузере или используйте флаг `-c cookies.txt` для curl.

### 4. Проверьте доступ к защищенному endpoint
```bash
curl -X GET http://localhost:5000/api/embedding-queue/status \
  -b cookies.txt
```

**Ответ (с авторизацией):**
```json
{
  "profilesInQueue": 0,
  "timestamp": "2024-12-06T..."
}
```

**Ответ (без авторизации - 401):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### 5. Получите информацию о текущем пользователе
```bash
curl -X GET http://localhost:5000/api/auth/me \
  -b cookies.txt
```

**Ответ:**
```json
{
  "userId": "guid-here",
  "email": "admin@vibeapp.com",
  "roles": ["Admin"]
}
```

### 6. Выйдите из системы
```bash
curl -X POST http://localhost:5000/api/auth/logout \
  -b cookies.txt
```

## Использование в Postman

### Импорт коллекции
1. Откройте Postman
2. Import → `docs/VibeApp-Auth-API.postman_collection.json`
3. Запустите запросы по порядку:
   - **1. Setup - Initialize Admin**
   - **2. Login as Admin**
   - **3. Get Current User Info**
   - **7. Admin - Get Embedding Queue Status**
   - **11. Logout**
   - **12. Test Unauthorized Access** (должен вернуть 401)

### Автоматические тесты
Каждый запрос в коллекции содержит тесты (Tests tab):
- Проверка статус кода
- Проверка структуры ответа
- Проверка наличия ролей

## Использование в Swagger

1. Откройте http://localhost:5000/swagger
2. Выполните `/api/auth/login` endpoint
3. Теперь cookie установлена - можете тестировать защищенные endpoints
4. На защищенных endpoints будет отображаться замок 🔒

## Регистрация обычных пользователей

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"User123"}'
```

**По умолчанию** новые пользователи **НЕ** имеют роли Admin.

### Назначение роли Admin
```bash
# Нужно быть залогиненным как Admin
curl -X POST http://localhost:5000/api/auth/assign-role \
  -H "Content-Type: application/json" \
  -b cookies.txt \
  -d '{"email":"user@example.com","role":"Admin"}'
```

### Удаление роли Admin
```bash
curl -X POST http://localhost:5000/api/auth/remove-role \
  -H "Content-Type: application/json" \
  -b cookies.txt \
  -d '{"email":"user@example.com","role":"Admin"}'
```

## Требования к паролю

- Минимум 3 символа
- Не требуются цифры, заглавные или строчные буквы
- Специальные символы НЕ обязательны

**Примеры валидных паролей:** `123`, `abc`, `admin`, `test`

## Production (Render.com)

### Первый запуск
После деплоя на Render:

1. Создайте Admin пользователя через API:
```bash
curl -X POST https://your-app.onrender.com/api/auth/init-admin \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@yourdomain.com","password":"SecurePassword123"}'
```

**Важно:** Endpoint `/api/auth/init-admin` доступен только в Development окружении!

В Production создайте первого admin через базу данных или измените код для одноразового доступа.

### Альтернатива - создание через SQL
```sql
-- Создайте пользователя через AspNetUsers
-- Назначьте роль через AspNetUserRoles
-- Подробности в документации ASP.NET Core Identity
```

## Безопасность в Production

Cookie настройки автоматически:
- ✅ `Secure=true` (только HTTPS)
- ✅ `HttpOnly=true` (защита от XSS)
- ✅ `SameSite=Strict` (защита от CSRF)

## Дополнительная документация

- **Postman коллекция:** `docs/VibeApp-Auth-API.postman_collection.json`
- **Примеры User Profile API:** `docs/POSTMAN_GUIDE.md`
- **OpenAI интеграция:** `docs/OPENAI_GATEWAY_EXAMPLE.md`
- **Deployment:** `docs/RENDER_DEPLOYMENT.md`

## Расширение системы

### Добавление новых ролей

```csharp
// В AuthController или startup коде
if (!await _roleManager.RoleExistsAsync("Manager"))
{
    await _roleManager.CreateAsync(new IdentityRole("Manager"));
}
```

### Защита endpoint новой ролью

```csharp
[HttpGet("manager-only")]
[Authorize(Roles = "Manager")]
public async Task<IActionResult> ManagerEndpoint()
{
    // Доступно только для Manager
}

[HttpGet("admin-or-manager")]
[Authorize(Roles = "Admin,Manager")]
public async Task<IActionResult> AdminOrManagerEndpoint()
{
    // Доступно для Admin ИЛИ Manager
}
```

## Troubleshooting

### Cookie не сохраняется в Postman
- Убедитесь, что в Settings → General включен "Cookie jar"
- Cookies автоматически сохраняются после `/api/auth/login`

### 401 Unauthorized на защищенных endpoints
- Проверьте что выполнили login
- Проверьте что cookie передается с запросом
- В браузере: проверьте DevTools → Application → Cookies

### 403 Forbidden
- Пользователь авторизован, но не имеет нужной роли
- Проверьте `/api/auth/me` для просмотра текущих ролей
- Назначьте роль через `/api/auth/assign-role`

### init-admin не работает в Production
- Endpoint доступен только в Development
- В Production создайте admin через базу данных или другой механизм

