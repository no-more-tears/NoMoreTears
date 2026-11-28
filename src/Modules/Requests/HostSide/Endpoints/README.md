## Endpoints

Документація ендпоінтів модуля Requests.

### Статуси

| Значення | Опис |
|--|--|
| `Draft` | Чернетка. Зміст можна редагувати, а заявку видалити |
| `Published` | Опублікована і чекає на прийняття до `acceptanceDeadline` (за замовчуванням 24 год) |
| `Expired` | Строк очікування сплив. Клієнт продовжує або скасовує заявку |
| `InProgress` | Майстер прийняв заявку і виконує її |
| `AwaitingConfirmation` | Майстер позначив роботу виконаною. Клієнт підтверджує або заперечує до `confirmationDeadline` (48 год) |
| `Completed` | Завершена: клієнт підтвердив або минуло 48 год без заперечень |
| `Failed` | Зірвана: неявка майстра (`NoShow`) або відмова майстра (`MasterRefusal`) |
| `Cancelled` | Скасована клієнтом |
| `Disputed` | Спірна: Trust прийняв скаргу. Після закриття інциденту заявка повертається в `statusBeforeDispute` |

### Переходи

| Дія | Хто | Зі статусу | У статус | Ендпоінт |
|--|--|--|--|--|
| `Created` | Client | | `Draft` | [1.1](#11-post-apirequests), [1.20](#120-post-apirequestsrequestidduplicate) |
| `Published` | Client | `Draft` | `Published` | [1.13](#113-post-apirequestsrequestidpublish) |
| `Expired` | System | `Published` | `Expired` | фонова задача після `acceptanceDeadline` |
| `Extended` | Client | `Expired` | `Published` | [1.14](#114-post-apirequestsrequestidextend) |
| `Accepted` | Master | `Published` | `InProgress` | [2.5](#25-post-apirequestsjobsrequestidaccept) |
| `MarkedAsDone` | Master | `InProgress` | `AwaitingConfirmation` | [2.6](#26-post-apirequestsjobsrequestidmark-as-done) |
| `CompletionConfirmed` | Client | `AwaitingConfirmation` | `Completed` | [1.16](#116-post-apirequestsrequestidconfirm-completion) |
| `CompletedAutomatically` | System | `AwaitingConfirmation` | `Completed` | фонова задача після `confirmationDeadline` |
| `CompletionObjected` | Client | `AwaitingConfirmation` | `InProgress` | [1.17](#117-post-apirequestsrequestidobject-to-completion) |
| `NoShowReported` | Client | `InProgress` | `Failed` | [1.18](#118-post-apirequestsrequestidreport-no-show) |
| `RefusedByMaster` | Master | `InProgress` | `Failed` | [2.7](#27-post-apirequestsjobsrequestidrefuse) |
| `Republished` | Client | `Failed` | `Published` | [1.19](#119-post-apirequestsrequestidrepublish) |
| `CancelledBeforeAcceptance` | Client | `Published`, `Expired` | `Cancelled` | [1.15](#115-post-apirequestsrequestidcancel) |
| `CancelledAfterAcceptance` | Client | `InProgress` | `Cancelled` | [1.15](#115-post-apirequestsrequestidcancel), лише з причиною |
| `Disputed` | System | `InProgress`, `AwaitingConfirmation`, `Completed`, `Failed` | `Disputed` | подія Trust `IncidentOpenedEvent` (скарга) |
| `DisputeSettled` | System | `Disputed` | попередній статус | подія Trust `IncidentClosedEvent` |

Кожен перехід записується в історію: час, ініціатор (`System`, `Client`, `Master`).

### Ліміт публікацій

- Клієнт має не більше ніж 3 одночасно опублікованих (`Published`) заявки. Перевищення повертає `422`.
- Обмеження Trust знижують ліміт до 1: санкція `PublicationRestriction` (крок 2) або тимчасовий захід з ефектом `PublicationLimit` (зловживання клієнта).
- Санкції `PublicationBlock` (крок 3) і `AccountBlock` забороняють публікацію до строку дії або зняття.
- Ліміт перевіряють публікація, продовження і повторна публікація.

### Завантаження фото

Фото приймаються асинхронно за схемою staging, як у модулі Masters:

1. Клієнт надсилає `multipart/form-data` (поле `file`). API зберігає файл у тимчасовий bucket і реєструє завантаження зі статусом `Pending`, а потім одразу відповідає `202 Accepted`. Заголовок `Location` вказує на [1.10](#110-get-apirequestsrequestiduploadsuploadid).
2. Consumer модуля (`Requests.HostSide.Consumers`) перевіряє сигнатуру вмісту для заявленого типу і розмір. Потім переносить файл у приватний bucket і додає його до чернетки.
3. Результат приходить нотифікацією `requests.media.completed` або `requests.media.failed`. Його також видно за статусом завантаження.

Обмеження: лише чернетка, до 10 фото на заявку (враховуються і ті, що ще перевіряються), `image/jpeg`, `image/png`, `image/webp`, `image/heic` до 25 MiB. Якщо заявку опублікували раніше, ніж фото пройшло перевірку, завантаження отримує статус `Failed`.

### Фонові задачі

Хост `Requests.HostSide.BackgroundJobs` раз на хвилину (`Requests:Jobs`) виконує дві операції:

- переводить у `Expired` опубліковані заявки зі сплилим `acceptanceDeadline`;
- завершує заявки `AwaitingConfirmation` зі сплилим `confirmationDeadline`.

## 1. Заявки клієнта

### 1.1 POST /api/requests

**Доступ:** Client

Створює чернетку. Адреса має бути геокодованою (текст і координати WGS 84), опис містить від 20 до 2000 символів, категорія є активною категорією каталогу.

#### Body

```json
{
  "categoryId": "01a188c6-ef8c-7f93-b4f5-66d07a531083",
  "address": {
    "value": "Львів, вул. Городоцька, 1, кв. 5",
    "coordinates": { "latitude": 49.8397, "longitude": 24.0297 }
  },
  "description": "Тече змішувач на кухні, потрібна заміна картриджа."
}
```

#### Response

`201 Created`, заголовок `Location: /api/requests/{requestId}`:

```json
{
  "id": "019a7a10-5f3e-7c11-9d2a-3b7e2f4c8a01",
  "clientId": "0d3a5e8e-0c3b-4c5e-9a3f-2f6f1f0b9a11",
  "categoryId": "01a188c6-ef8c-7f93-b4f5-66d07a531083",
  "address": {
    "value": "Львів, вул. Городоцька, 1, кв. 5",
    "coordinates": { "latitude": 49.8397, "longitude": 24.0297 }
  },
  "description": "Тече змішувач на кухні, потрібна заміна картриджа.",
  "status": "Draft",
  "publicationCount": 0,
  "isRepublished": false,
  "publishedAt": null,
  "acceptanceDeadline": null,
  "assignment": null,
  "confirmationDeadline": null,
  "completedAt": null,
  "failure": null,
  "cancelledAt": null,
  "cancellationReason": null,
  "incidentId": null,
  "statusBeforeDispute": null,
  "mediaFiles": [],
  "lastStatusChange": {
    "id": "019a7a10-5f3e-7c11-9d2a-3b7e2f4c8a02",
    "from": null,
    "to": "Draft",
    "transition": "Created",
    "initiatorRole": "Client",
    "initiatorId": "0d3a5e8e-0c3b-4c5e-9a3f-2f6f1f0b9a11",
    "reason": null,
    "occurredAt": "2026-11-12T14:00:00+00:00"
  },
  "createdAt": "2026-11-12T14:00:00+00:00",
  "lastUpdatedAt": null
}
```

Після прийняття `assignment` містить майстра і орієнтовний час прибуття. Після зриву `failure` містить тип і причину:

```json
{
  "assignment": {
    "masterId": "0b8f3c2e-6a1d-4e0f-8c7b-5d2a9e1f3b44",
    "acceptedAt": "2026-11-12T15:00:00+00:00",
    "expectedArrivalAt": "2026-11-12T17:00:00+00:00",
    "noShowReportableFrom": "2026-11-12T18:00:00+00:00"
  },
  "failure": { "kind": "MasterRefusal", "masterId": "0b8f3c2e-6a1d-4e0f-8c7b-5d2a9e1f3b44", "reason": "Немає потрібної деталі" }
}
```

### 1.2 GET /api/requests

**Доступ:** Client

Сторінка заявок поточного клієнта, спершу найновіші.

#### Query-параметри

| Параметр | Тип | Обов'язковий | Опис |
|--|--|--|--|
| `status` | string | ні | Фільтр за статусом, наприклад `Published` |
| `pageNumber` | int | ні | Номер сторінки, від 1 (за замовчуванням 1) |
| `pageSize` | int | ні | Розмір сторінки, 1–100 (за замовчуванням 20) |

#### Response

```json
{
  "items": [
    {
      "id": "019a7a10-5f3e-7c11-9d2a-3b7e2f4c8a01",
      "clientId": "0d3a5e8e-0c3b-4c5e-9a3f-2f6f1f0b9a11",
      "categoryId": "01a188c6-ef8c-7f93-b4f5-66d07a531083",
      "status": "InProgress",
      "publicationCount": 1,
      "publishedAt": "2026-11-12T14:10:00+00:00",
      "acceptanceDeadline": "2026-11-13T14:10:00+00:00",
      "masterId": "0b8f3c2e-6a1d-4e0f-8c7b-5d2a9e1f3b44",
      "expectedArrivalAt": "2026-11-12T17:00:00+00:00",
      "createdAt": "2026-11-12T14:00:00+00:00",
      "lastUpdatedAt": "2026-11-12T15:00:00+00:00"
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 1,
  "totalPages": 1,
  "hasNextPage": false
}
```

### 1.3 GET /api/requests/{requestId}

**Доступ:** Client

Повертає заявку у форматі [1.1](#11-post-apirequests).

### 1.4 GET /api/requests/{requestId}/history

**Доступ:** Client

Історія статусів у хронологічному порядку.

#### Response

```json
[
  {
    "id": "019a7a10-5f3e-7c11-9d2a-3b7e2f4c8a02",
    "from": null,
    "to": "Draft",
    "transition": "Created",
    "initiatorRole": "Client",
    "initiatorId": "0d3a5e8e-0c3b-4c5e-9a3f-2f6f1f0b9a11",
    "reason": null,
    "occurredAt": "2026-11-12T14:00:00+00:00"
  },
  {
    "id": "019a7a1b-0c41-7d22-8e3f-4a6b1c2d3e04",
    "from": "Draft",
    "to": "Published",
    "transition": "Published",
    "initiatorRole": "Client",
    "initiatorId": "0d3a5e8e-0c3b-4c5e-9a3f-2f6f1f0b9a11",
    "reason": null,
    "occurredAt": "2026-11-12T14:10:00+00:00"
  }
]
```

### 1.5 PUT /api/requests/{requestId}/category

**Доступ:** Client

Змінює категорію чернетки. Архівовану категорію обрати не можна.

#### Body

```json
{
  "categoryId": "01a188c6-ef8c-7f93-b4f5-66d07a531083"
}
```

### 1.6 PUT /api/requests/{requestId}/address

**Доступ:** Client

Змінює адресу чернетки. Body має формат поля `address` з [1.1](#11-post-apirequests):

```json
{
  "value": "Львів, вул. Шевченка, 10",
  "coordinates": { "latitude": 49.8441, "longitude": 24.0262 }
}
```

### 1.7 PUT /api/requests/{requestId}/description

**Доступ:** Client

Змінює опис чернетки (від 20 до 2000 символів).

#### Body

```json
{
  "description": "Тече змішувач на кухні та у ванній, потрібна заміна обох картриджів."
}
```

### 1.8 DELETE /api/requests/{requestId}

**Доступ:** Client

Видаляє чернетку і повертає `204 No Content`. Опубліковану заявку видалити не можна, її можна лише скасувати ([1.15](#115-post-apirequestsrequestidcancel)). Фото видаляються зі сховища після фіксації зміни.

### 1.9 POST /api/requests/{requestId}/media

**Доступ:** Client

Приймає фото чернетки (`multipart/form-data`, поле `file`). Файл перевіряється асинхронно, див. [Завантаження фото](#завантаження-фото).

#### Response

`202 Accepted`, заголовок `Location: /api/requests/{requestId}/uploads/{uploadId}`:

```json
{
  "id": "019a7a12-77aa-7e01-b1c2-d3e4f5a6b7c8",
  "requestId": "019a7a10-5f3e-7c11-9d2a-3b7e2f4c8a01",
  "fileName": "mixer.jpg",
  "contentType": "image/jpeg",
  "sizeInBytes": 734003,
  "status": "Pending",
  "failureReason": null,
  "mediaFileId": null,
  "createdAt": "2026-11-12T14:02:00+00:00",
  "processedAt": null
}
```

### 1.10 GET /api/requests/{requestId}/uploads/{uploadId}

**Доступ:** Client

Повертає статус завантаження у форматі відповіді [1.9](#19-post-apirequestsrequestidmedia). Для `Failed` поле `failureReason` містить причину, а для `Completed` поле `mediaFileId` вказує на фото заявки.

### 1.11 GET /api/requests/{requestId}/media/{mediaFileId}

**Доступ:** Client

Повертає вміст фото (потік зображення).

### 1.12 DELETE /api/requests/{requestId}/media/{mediaFileId}

**Доступ:** Client

Відкріплює фото від чернетки і повертає заявку. Файл видаляється зі сховища після фіксації зміни.

### 1.13 POST /api/requests/{requestId}/publish

**Доступ:** Client

Публікує чернетку. З цього моменту зміст фіксується, а заявка чекає на прийняття до `acceptanceDeadline`. Перевіряються ліміт публікацій (див. [Ліміт публікацій](#ліміт-публікацій)) і активність категорії.

### 1.14 POST /api/requests/{requestId}/extend

**Доступ:** Client

Продовжує очікування прийняття простроченої заявки (`Expired`). Новий строк відраховується від моменту продовження, ліміт публікацій перевіряється знову.

### 1.15 POST /api/requests/{requestId}/cancel

**Доступ:** Client

Скасовує заявку. До прийняття (`Published`, `Expired`) причина необов'язкова, і скасування не має наслідків. Після прийняття (`InProgress`) причина обов'язкова, а скасування враховується в сигналах зловживань Trust.

#### Body

```json
{
  "reason": "Проблему вирішено самостійно"
}
```

Body можна не надсилати, якщо причина не потрібна.

### 1.16 POST /api/requests/{requestId}/confirm-completion

**Доступ:** Client

Підтверджує завершення роботи, яку майстер позначив виконаною.

### 1.17 POST /api/requests/{requestId}/object-to-completion

**Доступ:** Client

Заперечує завершення, і заявка повертається у виконання (`InProgress`). Майстер отримує сповіщення з причиною.

#### Body

```json
{
  "reason": "Змішувач досі тече"
}
```

### 1.18 POST /api/requests/{requestId}/report-no-show

**Доступ:** Client

Фіксує неявку майстра. Дозволено лише після `assignment.noShowReportableFrom`, тобто після орієнтовного часу прибуття і буфера 60 хв.

### 1.19 POST /api/requests/{requestId}/republish

**Доступ:** Client

Повторно публікує зірвану заявку (`Failed`) з підвищеним пріоритетом у скорингу. Попереднє призначення й зрив очищуються, `publicationCount` збільшується.

### 1.20 POST /api/requests/{requestId}/duplicate

**Доступ:** Client

Створює нову чернетку за адресою та категорією будь-якої власної заявки. Фото не копіюються. Відповідь така сама, як у [1.1](#11-post-apirequests).

#### Body

```json
{
  "description": "Знову тече змішувач на кухні, потрібен огляд."
}
```

## 2. Заявки майстра

Ендпоінти доступні ролі `Master`. Identity видає її автоматично після верифікації профілю майстра в модулі Masters.

### 2.1 GET /api/requests/jobs

**Доступ:** Master

Сторінка заявок, прийнятих поточним майстром, спершу останні прийняті. Query-параметри та відповідь такі самі, як у [1.2](#12-get-apirequests).

### 2.2 GET /api/requests/jobs/{requestId}

**Доступ:** Master

Повертає заявку у форматі [1.1](#11-post-apirequests):

- заявку, прийняту майстром, з адресою (адреса залишається видимою в статусах `InProgress`, `AwaitingConfirmation`, `Completed`, `Disputed`);
- опубліковану заявку, яку майстер може прийняти, без адреси (`address: null`).

Майстер може прийняти заявку, лише якщо він верифікований, має верифіковану зону обслуговування і підтверджену категорію заявки. Також майстер не має бути призупинений. В інших випадках ендпоінт повертає `404`.

### 2.3 GET /api/requests/jobs/{requestId}/history

**Доступ:** Master

Історія статусів заявки, прийнятої майстром, у форматі [1.4](#14-get-apirequestsrequestidhistory).

### 2.4 GET /api/requests/jobs/{requestId}/media/{mediaFileId}

**Доступ:** Master

Повертає вміст фото заявки, яку майстер може бачити за правилами [2.2](#22-get-apirequestsjobsrequestid).

### 2.5 POST /api/requests/jobs/{requestId}/accept

**Доступ:** Master

Приймає опубліковану заявку до спливу `acceptanceDeadline`. Майстер обирає орієнтовний час прибуття: 2, 6 або 24 години. Після прийняття майстру розкривається адреса, а клієнт отримує сповіщення. Якщо заявку вже прийняв інший майстер, ендпоінт повертає `409` або `422`.

#### Body

```json
{
  "arrivalIntervalInHours": 2
}
```

### 2.6 POST /api/requests/jobs/{requestId}/mark-as-done

**Доступ:** Master

Позначає роботу виконаною. Клієнт підтверджує завершення, або воно настає автоматично через 48 год за відсутності заперечень чи скарги.

### 2.7 POST /api/requests/jobs/{requestId}/refuse

**Доступ:** Master

Фіксує відмову від виконання з обов'язковою причиною. Заявка стає зірваною, клієнт може повторно її опублікувати.

#### Body

```json
{
  "reason": "Немає потрібної деталі"
}
```

## 3. Арбітраж

### 3.1 GET /api/requests/support

**Доступ:** PlatformSupport, PlatformAdministrator

Сторінка всіх заявок, спершу останні змінені. Відповідь така сама, як у [1.2](#12-get-apirequests).

#### Query-параметри

| Параметр | Тип | Обов'язковий | Опис |
|--|--|--|--|
| `status` | string | ні | Фільтр за статусом, наприклад `Disputed` |
| `clientId` | uuid | ні | Заявки клієнта |
| `masterId` | uuid | ні | Заявки, прийняті майстром |
| `pageNumber` | int | ні | Номер сторінки, від 1 (за замовчуванням 1) |
| `pageSize` | int | ні | Розмір сторінки, 1–100 (за замовчуванням 20) |

### 3.2 GET /api/requests/support/{requestId}

**Доступ:** PlatformSupport, PlatformAdministrator

Повна заявка з адресою у форматі [1.1](#11-post-apirequests).

### 3.3 GET /api/requests/support/{requestId}/history

**Доступ:** PlatformSupport, PlatformAdministrator

Історія статусів у форматі [1.4](#14-get-apirequestsrequestidhistory).

### 3.4 GET /api/requests/support/{requestId}/media/{mediaFileId}

**Доступ:** PlatformSupport, PlatformAdministrator

Повертає вміст фото заявки.