# Migration: AddAdminRole

## Описание

Миграция автоматически создает роль "Admin" и назначает ее пользователю с email `admin@vibe-app.com`.

## Файл миграции

`src/VibeApp.Data/Migrations/20251206105331_AddAdminRole.cs`

## Что делает миграция

### При применении (Up):

1. **Создает роль Admin** (если не существует):
   - Добавляет запись в таблицу `AspNetRoles`
   - Name: `Admin`
   - NormalizedName: `ADMIN`

2. **Назначает роль пользователю** `admin@vibe-app.com` (если пользователь существует):
   - Ищет пользователя в `AspNetUsers`
   - Находит роль Admin в `AspNetRoles`
   - Добавляет связь в `AspNetUserRoles`

3. **Idempotent операции**:
   - Можно применять несколько раз
   - Не будет ошибок при повторном применении
   - Проверяет существование перед вставкой

### При откате (Down):

1. Удаляет связь пользователя с ролью Admin
2. Удаляет роль Admin

## Использование

### Локальная разработка

**Шаг 1:** Создайте пользователя с email `admin@vibe-app.com`

Через веб-интерфейс:
```
1. Откройте http://localhost:5000/Account/Register
2. Email: admin@vibe-app.com
3. Password: admin (или любой пароль минимум 3 символа)
```

Или через API:
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@vibe-app.com","password":"admin"}'
```

**Шаг 2:** Примените миграцию
```bash
cd src/VibeApp.Api
dotnet ef database update
```

**Шаг 3:** Проверьте результат
```bash
# Войдите в систему
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -c cookies.txt \
  -d '{"email":"admin@vibe-app.com","password":"admin"}'

# Проверьте роли
curl -X GET http://localhost:5000/api/auth/me -b cookies.txt
```

**Ожидаемый результат:**
```json
{
  "userId": "...",
  "email": "admin@vibe-app.com",
  "roles": ["Admin"]
}
```

### Production (Render.com)

Миграция применяется **автоматически** при деплое благодаря коду в `Program.cs`:

```csharp
if (!app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetService<AppDbContext>();
    if (db != null)
    {
        db.Database.Migrate(); // ← Здесь применяется миграция
    }
}
```

**Важно:** Создайте пользователя `admin@vibe-app.com` ПЕРЕД деплоем или сразу после:

1. Откройте `https://your-app.onrender.com/Account/Register`
2. Зарегистрируйте `admin@vibe-app.com`
3. Перезапустите сервис на Render (миграция применится при старте)

Или используйте API:
```bash
curl -X POST https://your-app.onrender.com/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@vibe-app.com","password":"admin123"}'
```

Затем перезапустите сервис на Render Dashboard.

## Откат миграции

Если нужно откатить миграцию:

```bash
cd src/VibeApp.Api
dotnet ef database update 20251206102734_AddRetryLogicToEmbeddingQueue
```

Это удалит роль Admin и связь с пользователем.

## Проверка в базе данных

### Проверить роль Admin:
```sql
SELECT * FROM "AspNetRoles" WHERE "Name" = 'Admin';
```

### Проверить пользователя:
```sql
SELECT * FROM "AspNetUsers" WHERE "Email" = 'admin@vibe-app.com';
```

### Проверить назначение роли:
```sql
SELECT 
    u."Email",
    r."Name" as "Role"
FROM "AspNetUsers" u
JOIN "AspNetUserRoles" ur ON u."Id" = ur."UserId"
JOIN "AspNetRoles" r ON ur."RoleId" = r."Id"
WHERE u."Email" = 'admin@vibe-app.com';
```

## Изменение email админа

Если хотите использовать другой email вместо `admin@vibe-app.com`:

### Вариант 1: Создайте новую миграцию

1. Удалите текущую pending миграцию:
```bash
cd src/VibeApp.Api
dotnet ef migrations remove
```

2. Отредактируйте миграцию с новым email
3. Создайте миграцию заново

### Вариант 2: Используйте API

После применения миграции используйте endpoint для назначения роли:

```bash
# Залогиньтесь как существующий admin
curl -X POST http://localhost:5000/api/auth/login \
  -c cookies.txt \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@vibe-app.com","password":"admin"}'

# Назначьте роль другому пользователю
curl -X POST http://localhost:5000/api/auth/assign-role \
  -b cookies.txt \
  -H "Content-Type: application/json" \
  -d '{"email":"newadmin@example.com","role":"Admin"}'
```

## Troubleshooting

### Миграция не назначила роль

**Причина:** Пользователь `admin@vibe-app.com` не существовал на момент применения миграции.

**Решение:** 
1. Создайте пользователя
2. Примените миграцию повторно (она idempotent):
```bash
dotnet ef database update --force
```

Или используйте API для назначения роли вручную.

### Ошибка "role Admin already exists"

Это нормально - миграция проверяет существование роли перед созданием.

### Пользователь есть, но роль не назначена

Проверьте email - должен быть точно `admin@vibe-app.com` (lowercase).

Или назначьте роль вручную через SQL:
```sql
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT 
    u."Id",
    r."Id"
FROM "AspNetUsers" u, "AspNetRoles" r
WHERE u."Email" = 'admin@vibe-app.com'
  AND r."Name" = 'Admin'
  AND NOT EXISTS (
    SELECT 1 FROM "AspNetUserRoles" ur
    WHERE ur."UserId" = u."Id" AND ur."RoleId" = r."Id"
  );
```

## Дополнительная информация

- **Основная документация:** `docs/AUTHORIZATION_GUIDE.md`
- **Быстрый старт:** `docs/QUICKSTART_AUTH.md`
- **README:** `README.md` (раздел "Создание первого Admin пользователя")

