# Embedding Queue API Guide

## Обзор

Система асинхронной генерации embeddings для профилей пользователей. При создании или обновлении профиля, его ID добавляется в очередь, и фоновый сервис автоматически обрабатывает очередь, генерируя embeddings через OpenAI API.

## Архитектура

### Компоненты

1. **EmbeddingQueue** (Entity)
   - Таблица в БД для хранения очереди профилей
   - Поля: 
     - Id - первичный ключ
     - UserProfileId - ссылка на профиль пользователя
     - CreatedAt - время добавления в очередь
     - RetryCount - счетчик попыток обработки (0-2, max 3 попытки)
     - LastProcessedAt - время последней попытки обработки

2. **EmbeddingQueueService**
   - Управление очередью: добавление, извлечение batch, удаление, очистка
   - Проверяет дубликаты перед добавлением в очередь
   - `DequeueBatchAsync` - извлекает batch записей с RetryCount < 3 (не удаляет из очереди)
   - `RequeueWithRetryAsync` - переставляет элемент в конец очереди с увеличенным RetryCount

3. **EmbeddingProcessingService** (BackgroundService)
   - Фоновый сервис, мониторящий очередь
   - Проверяет каждые 5 секунд
   - Обрабатывает несколько профилей параллельно (настраивается)
   - **Retry-логика с ограничением попыток:**
     - При успехе: удаляет из очереди (`RemoveFromQueueAsync`)
     - При ошибке: переставляет в конец очереди (`RequeueWithRetryAsync`)
     - Максимум 3 попытки обработки (RetryCount: 0, 1, 2)
     - После 3-х неудачных попыток элемент остается в БД, но не обрабатывается
   - Логирует успехи, ошибки и requeue операции

4. **UserProfileService** (обновлен)
   - При создании/обновлении профиля добавляет ID в очередь
   - Больше не генерирует embedding синхронно

### Процесс работы

```
1. POST /api/userprofile/import
   └─> UserProfileService.CreateNewProfile()
       └─> EmbeddingQueueService.EnqueueProfileAsync(profileId)
           └─> Добавляет в таблицу EmbeddingQueues

2. EmbeddingProcessingService (фоновый процесс)
   └─> Каждые 5 секунд проверяет очередь
       └─> EmbeddingQueueService.DequeueBatchAsync(batchSize)
           └─> Извлекает batch (напр. 5 профилей)
               └─> Для каждого профиля параллельно:
                   └─> UserProfileEmbeddingService.GenerateAndSaveEmbeddingAsync()
                       └─> OpenAIGateway.GetEmbeddingAsync() → сохраняет в БД
                           └─> При успехе: RemoveFromQueueAsync(queueItemId)
                           └─> При ошибке: RequeueWithRetryAsync(queueItemId)
                               └─> Увеличивает RetryCount, обновляет LastProcessedAt
                               └─> Обновляет CreatedAt (элемент уходит в конец очереди)
                               └─> Если RetryCount >= 3: элемент больше не выбирается
```

## API Endpoints

### 1. Статус очереди

**GET** `/api/embedding-queue/status`

Возвращает количество профилей в очереди.

**Ответ:**
```json
{
  "profilesInQueue": 15,
  "timestamp": "2025-12-05T22:35:00Z"
}
```

**Пример:**
```bash
curl http://localhost:5000/api/embedding-queue/status
```

### 2. Очистить очередь

**POST** `/api/embedding-queue/clear`

Удаляет все записи из очереди.

**Ответ:**
```json
{
  "message": "Queue cleared successfully",
  "itemsRemoved": 15,
  "timestamp": "2025-12-05T22:35:00Z"
}
```

**Пример:**
```bash
curl -X POST http://localhost:5000/api/embedding-queue/clear
```

## Использование

### Импорт профилей с автоматической генерацией embeddings

```bash
curl -X POST http://localhost:5000/api/userprofile/import \
  -H "Content-Type: application/json" \
  -d @sample-user-profiles-batch.json
```

После импорта профили автоматически добавляются в очередь. Фоновый сервис начнет их обработку.

### Мониторинг очереди

```bash
# Проверить статус
curl http://localhost:5000/api/embedding-queue/status

# Если нужно остановить обработку - очистить очередь
curl -X POST http://localhost:5000/api/embedding-queue/clear
```

## Логирование

Фоновый сервис логирует все действия:

```
[Information] Embedding Processing Service started with 5 concurrent tasks
[Information] Processing batch of 3 profiles
[Information] Processing embedding for profile ID: 123 (Queue Item: 45)
[Information] Successfully completed processing for profile ID: 123
[Error] Error generating embedding for profile ID: 456. Requeuing with retry.
[Information] Profile ID: 456 has been requeued for retry
[Information] Processing embedding for profile ID: 456 (Queue Item: 46) [Retry 1]
[Information] Successfully completed processing for profile ID: 456
```

Логи можно просматривать в консоли приложения или в настроенной системе логирования.

## Производительность

- **Обработка**: Несколько профилей параллельно (по умолчанию 5)
- **Интервал проверки**: 5 секунд (когда очередь пуста)
- **Batch processing**: Извлекает batch записей и обрабатывает все параллельно
- **Retry логика**: 
  - Автоматический retry при ошибках
  - Максимум 3 попытки на профиль
  - Проблемные профили уходят в конец очереди
  - После 3 неудачных попыток профиль перестает обрабатываться
- **Дубликаты**: предотвращаются автоматически
- **Настраиваемость**: Количество параллельных задач настраивается в `appsettings.json`

## Преимущества

✅ **Асинхронность** - API быстро отвечает, не ждет генерации embedding
✅ **Надежность** - ошибки не блокируют импорт профилей, автоматический retry
✅ **Масштабируемость** - очередь может накапливать задачи
✅ **Контроль** - API для мониторинга и управления
✅ **Логирование** - полная прозрачность процесса
✅ **Защита от зацикливания** - максимум 3 попытки, проблемные элементы не блокируют очередь

## Настройка

### Изменить количество параллельных задач

В файле `appsettings.json` или `appsettings.Development.json`:

```json
{
  "EmbeddingProcessing": {
    "ConcurrentTasks": 10
  }
}
```

**Рекомендации:**
- По умолчанию: 5 параллельных задач
- Для локальной разработки: 2-5 задач
- Для production с высокой нагрузкой: 10-20 задач
- Учитывайте rate limits OpenAI API (tier-based)
- Учитывайте нагрузку на БД

### Изменить интервал проверки очереди

В файле `EmbeddingProcessingService.cs`:

```csharp
// Изменить с 5 секунд на другое значение
await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
```

## Troubleshooting

### Профили не обрабатываются

1. Проверьте, что фоновый сервис запущен (логи при старте)
2. Проверьте статус очереди: `GET /api/embedding-queue/status`
3. Проверьте логи на ошибки

### Слишком медленная обработка

- Проверьте лимиты OpenAI API
- Увеличьте параллелизм (с осторожностью)
- Проверьте производительность БД

### Ошибки генерации

- Проверьте переменную окружения `OPENAI_API_KEY`
- Проверьте баланс OpenAI аккаунта
- Проверьте логи для деталей ошибки
- Профили с повторяющимися ошибками (3+ попыток) остаются в БД с RetryCount >= 3
- Для повторной обработки проблемных профилей можно:
  - Очистить очередь и пересоздать элементы с RetryCount = 0
  - Вручную обновить RetryCount в БД для конкретных профилей

### Профили застряли с RetryCount = 3

Если профили не обрабатываются из-за достижения лимита попыток:

1. Проверьте логи - почему обработка падала
2. Исправьте проблему (API key, баланс, формат данных)
3. Сбросьте RetryCount в БД:

```sql
UPDATE "EmbeddingQueues" SET "RetryCount" = 0, "CreatedAt" = NOW() WHERE "RetryCount" >= 3;
```

Или очистите очередь и переимпортируйте профили.

