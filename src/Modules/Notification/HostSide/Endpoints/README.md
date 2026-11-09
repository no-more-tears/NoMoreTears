## Endpoints

Документація модуля Notification: HTTP-ендпоінти сповіщень (WebApi) і real-time канал SignalR (окремий хост `Notification.HostSide.SignalR`).

### Модель сповіщення

```json
{
  "id": "0193f1c2-4b7a-7c1e-9d2f-5a6b7c8d9e0f",
  "type": "masters.verification.rejected",
  "category": "Verification",
  "severity": "Warning",
  "title": "Верифікацію відхилено",
  "body": "Причина: документ нечитабельний. Виправте профіль і надішліть його повторно.",
  "data": {
    "masterId": "0d3a5e8e-0c3b-4c5e-9a3f-2f6f1f0b9a11",
    "reason": "документ нечитабельний"
  },
  "link": "/master/profile/verification",
  "occurredAt": "2026-11-09T14:20:00+00:00",
  "createdAt": "2026-11-09T14:20:01+00:00",
  "deliveredAt": null,
  "readAt": null,
  "isRead": false
}
```

| Поле | Опис |
|--|--|
| `type` | Стабільний код. Клієнт може показувати власний текст і дії за ним |
| `category` | `Account`, `Verification`, `Profile`, `Sanction`, `Request`, `Dispute`, `System` |
| `severity` | `Info`, `Success`, `Warning`, `Critical` |
| `title`, `body` | Готовий текст у культурі `Notification:InApp:Culture` (за замовчуванням `uk`) |
| `data` | Значення, потрібні клієнту (ідентифікатори, причина, дати у форматі `yyyy-MM-dd HH:mm UTC`) |
| `link` | Відносний маршрут фронтенду до предмета сповіщення |
| `deliveredAt` | Момент, коли клієнт підтвердив отримання в реальному часі |
| `readAt` | Момент прочитання |

### Типи сповіщень

| `type` | `data` |
|--|--|
| `masters.verification.submitted` | `masterId` |
| `masters.verification.verified` | `masterId` |
| `masters.verification.rejected` | `masterId`, `reason` |
| `masters.verification.revoked` | `masterId`, `sanctionId`, `reverificationAllowedFrom` |
| `masters.verification.restored` | `masterId`, `sanctionId` |
| `masters.category.verified` | `masterId`, `categoryId` |
| `masters.category.rejected` | `masterId`, `categoryId`, `reason` |
| `masters.service-area.verified` | `masterId` |
| `masters.service-area.rejected` | `masterId`, `reason` |
| `masters.verification-hold.placed` | `masterId`, `reason` |
| `masters.verification-hold.released` | `masterId` |
| `masters.sanction.applied` | `masterId`, `sanctionId`, `kind`, `expiresAt` (`UntilLifted`, якщо строку немає) |
| `masters.sanction.lifted` | `masterId`, `sanctionId`, `kind` |
| `masters.upload.completed` | `masterId`, `uploadId`, `purpose`, `fileName`, `categoryId`, `documentId` |
| `masters.upload.failed` | `masterId`, `uploadId`, `purpose`, `fileName`, `reason` |

## 1. HTTP (WebApi)

Усі ендпоінти потребують автентифікації i працюють лише зі сповіщеннями поточного користувача. Чуже або видалене сповіщення повертає `404`.

### 1.1 GET /api/notifications

**Доступ:** Authenticated

Сторінка сповіщень, спершу найновіші.

#### Query-параметри

| Параметр | Тип | Обов'язковий | Опис |
|--|--|--|--|
| `unreadOnly` | bool | ні | Лише непрочитані (за замовчуванням `false`) |
| `category` | string | ні | Фільтр за категорією, наприклад `Verification` |
| `pageNumber` | int | ні | Номер сторінки, від 1 (за замовчуванням 1) |
| `pageSize` | int | ні | Розмір сторінки, 1–100 (за замовчуванням 20) |

#### Response

```json
{
  "items": [ { "id": "0193f1c2-4b7a-7c1e-9d2f-5a6b7c8d9e0f", "type": "masters.verification.verified", "...": "..." } ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 1,
  "totalPages": 1,
  "hasNextPage": false
}
```

### 1.2 GET /api/notifications/unread-count

**Доступ:** Authenticated

Кількість непрочитаних сповіщень.

#### Response

```json
{
  "count": 3
}
```

### 1.3 POST /api/notifications/{notificationId}/read

**Доступ:** Authenticated

Позначає сповіщення прочитаним і повертає його. Повторний виклик нічого не змінює. Інші пристрої отримують `NotificationsChanged` і `UnreadCountChanged`.

### 1.4 POST /api/notifications/read

**Доступ:** Authenticated

Позначає прочитаними кілька сповіщень (до 100 за раз).

#### Body

```json
{
  "notificationIds": [
    "0193f1c2-4b7a-7c1e-9d2f-5a6b7c8d9e0f",
    "0193f1c2-4b7a-7c1e-9d2f-5a6b7c8d9e10"
  ]
}
```

#### Response

```json
{
  "change": "Read",
  "notificationIds": ["0193f1c2-4b7a-7c1e-9d2f-5a6b7c8d9e0f"],
  "affectedCount": 1,
  "unreadCount": 2,
  "occurredAt": "2026-11-09T14:30:00+00:00"
}
```

`notificationIds` у відповіді містить лише ті сповіщення, які справді змінилися.

### 1.5 POST /api/notifications/read-all

**Доступ:** Authenticated

Позначає прочитаними всі сповіщення користувача. Відповідь має формат [1.4](#14-post-apinotificationsread) зі значенням `change` = `ReadAll` і порожнім `notificationIds`.

### 1.6 DELETE /api/notifications/{notificationId}

**Доступ:** Authenticated

Видаляє сповіщення з історії (м'яке видалення). Відповідь має формат [1.4](#14-post-apinotificationsread) зі значенням `change` = `Deleted`.

## 2. SignalR

### 2.1 Підключення

- **URL:** `{signalr-host}/hubs/notifications` (локально `https://localhost:7090/hubs/notifications`, у Docker `http://localhost:5090/hubs/notifications`).
- **Автентифікація:** access-токен Identity у query-параметрі `access_token`, бо браузери не передають заголовки у WebSocket. Токени відкликаних сесій відхиляються.
- **Протоколи:** MessagePack (рекомендовано) або JSON.
- **Keep-alive:** сервер надсилає ping кожні 15 с, а з'єднання без повідомлень від клієнта закривається через 30 с. Клієнту варто ввімкнути автоматичне перепідключення.

```ts
import { HubConnectionBuilder } from "@microsoft/signalr";
import { MessagePackHubProtocol } from "@microsoft/signalr-protocol-msgpack";

const connection = new HubConnectionBuilder()
  .withUrl(`${signalRUrl}/hubs/notifications`, { accessTokenFactory: () => accessToken })
  .withHubProtocol(new MessagePackHubProtocol())
  .withAutomaticReconnect()
  .build();

connection.on("ReceiveNotification", async (notification) => {
  show(notification);
  await connection.invoke("Acknowledge", [notification.id]);
});
connection.on("ReceiveNotifications", async (notifications) => {
  notifications.forEach(show);
  await connection.invoke("Acknowledge", notifications.map((n) => n.id));
});
connection.on("NotificationsChanged", (change) => applyChange(change));
connection.on("UnreadCountChanged", ({ count }) => setBadge(count));

await connection.start();
```

### 2.2 Методи сервер → клієнт

| Метод | Аргумент | Коли |
|--|--|--|
| `ReceiveNotifications` | масив сповіщень | Одразу після підключення: усі сповіщення, отримання яких клієнт не підтвердив (до 100, найстаріші першими) |
| `ReceiveNotification` | сповіщення | Нове сповіщення |
| `UnreadCountChanged` | `{ "count": 3 }` | Після підключення, нового сповіщення та будь-якої зміни |
| `NotificationsChanged` | відповідь [1.4](#14-post-apinotificationsread) | Сповіщення прочитано або видалено на іншому пристрої |

Доставка відбувається щонайменше один раз, тому клієнт має відкидати дублікати за `id`.

### 2.3 Методи клієнт → сервер

| Метод | Аргумент | Повертає | Опис |
|--|--|--|--|
| `Acknowledge` | `Guid[]` (до 100) | кількість підтверджених | Підтверджує отримання; непідтверджені сповіщення надсилаються знову при наступному підключенні |