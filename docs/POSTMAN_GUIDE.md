# Postman Collection - Инструкция

## Импорт коллекции в Postman

1. Откройте Postman
2. Нажмите **Import** (левый верхний угол)
3. Выберите файл `VibeApp-UserProfile-API.postman_collection.json`
4. Коллекция появится в списке коллекций

## Настройка переменных

Коллекция использует переменную `{{base_url}}` для адреса API.

**По умолчанию**: `http://localhost:5000`

### Как изменить base_url:

1. Откройте коллекцию в Postman
2. Перейдите на вкладку **Variables**
3. Измените значение `base_url` (например, на `https://your-app.onrender.com`)
4. Нажмите **Save**

## Запросы в коллекции

### 1. Import User Profiles (Batch)
**POST** `/api/userprofile/import`

Импортирует массив профилей и синхронизирует с БД.

⚠️ **ВАЖНО**: Удаляет пользователей, которых нет в списке!

**Пример ответа:**
```json
{
  "totalProcessed": 2,
  "created": 1,
  "updated": 1,
  "deleted": 0,
  "errors": [],
  "message": "Successfully processed 2 profiles..."
}
```

### 2. Get All User Profiles
**GET** `/api/userprofile`

Получает все профили со всеми связанными данными.

### 3. Get User Profile by ID
**GET** `/api/userprofile/{id}`

Получает конкретный профиль по ID.

Замените `1` в URL на нужный ID.

### 4. Update User Profile
**PUT** `/api/userprofile/{id}`

Обновляет существующий профиль.

### 5. Delete User Profile
**DELETE** `/api/userprofile/{id}`

Удаляет профиль и все связанные данные.

### 6. Health Check
**GET** `/health`

Проверка работоспособности API.

## Пример последовательности тестирования

1. **Health Check** - убедитесь, что API работает
2. **Import User Profiles** - импортируйте тестовые данные
3. **Get All User Profiles** - проверьте, что данные созданы
4. **Get User Profile by ID** - получите конкретный профиль
5. **Update User Profile** - обновите профиль
6. **Get User Profile by ID** - проверьте обновление
7. **Import User Profiles** (с одним пользователем) - проверьте синхронизацию (удаление)
8. **Get All User Profiles** - проверьте, что остался только один пользователь

## Советы

- Используйте **Environment** в Postman для разных окружений (Local, Staging, Production)
- Сохраняйте ID созданных профилей в переменных для удобства тестирования
- Проверяйте статус ответов: 200 (OK), 404 (Not Found), 500 (Error)

## Troubleshooting

**Ошибка подключения:**
- Убедитесь, что приложение запущено
- Проверьте, что `base_url` указывает на правильный адрес
- Для локального тестирования: `http://localhost:5000`

**404 Not Found:**
- Проверьте, что профиль с указанным ID существует
- Используйте GET All для получения списка ID

**500 Internal Server Error:**
- Проверьте логи приложения
- Убедитесь, что PostgreSQL доступна
- Проверьте формат JSON в запросе


