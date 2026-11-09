## Endpoints

Документація ендпоінтів модуля Masters: профіль майстра, верифікація, каталог категорій.

### Загальні відомості

- Ідентифікатор профілю майстра дорівнює ідентифікатору користувача (`sub` з access-токена), тому один акаунт має один профіль.
- Ендпоінти `/api/masters/me/*` доступні ролі `Worker` і змінюють лише профіль поточного користувача.
- Ендпоінти перевірки профілів доступні ролям `PlatformSupport` і `PlatformAdministrator`, каталог змінює лише `PlatformAdministrator`.
- Усі ендпоінти мають спільний ліміт частоти запитів API.
- Одночасна зміна того самого профілю (наприклад, майстром і санкцією Trust) повертає `409 Conflict`. Дані треба перечитати й повторити дію.

### Статуси

| Поле | Значення |
|--|--|
| `verification` | `NotSubmitted`, `Pending` (на верифікації), `Verified`, `Rejected`, `Revoked` (позбавлено верифікації) |
| `serviceAreaStatus`, `categories[].status` | `Pending`, `Verified`, `Rejected` |
| `sanctions[].kind` | `Warning`, `Restriction`, `Suspension`, `Deverification` |
| `uploads.status` | `Pending`, `Completed`, `Failed` |
| `uploads.purpose` | `Avatar`, `IdentityDocument`, `QualificationDocument` |

### Завантаження файлів

Файли приймаються асинхронно за схемою staging:

1. Клієнт надсилає `multipart/form-data`. API зберігає файл у тимчасовий bucket і реєструє завантаження зі статусом `Pending`, а потім одразу відповідає `202 Accepted`. Заголовок `Location` вказує на [1.14](#114-get-apimastersmeuploadsuploadid).
2. Consumer модуля (`Masters.HostSide.Consumers`) перевіряє файл: сигнатуру вмісту для заявленого типу і розмір. Потім переносить файл у приватний bucket і додає до профілю.
3. Результат приходить нотифікацією `masters.upload.completed` або `masters.upload.failed`. Його також видно за статусом завантаження.

Обмеження: фото профілю `image/jpeg`, `image/png`, `image/webp` до 5 MiB; документи `application/pdf`, `image/jpeg`, `image/png` до 25 MiB.

## 1. Профіль майстра

### 1.1 POST /api/masters

**Доступ:** Worker

Створює профіль майстра поточного користувача. Зона обслуговування задається або колом (`center` + `radiusInKilometers` до 100 км), або полігоном (`boundary`, від 3 до 100 вершин). Часовий пояс графіка задається ідентифікатором IANA.

#### Body

```json
{
  "firstName": "Іван",
  "lastName": "Франко",
  "categoryIds": ["01a188c6-ef8c-7f93-b4f5-66d07a531083"],
  "serviceArea": {
    "center": { "latitude": 49.8397, "longitude": 24.0297 },
    "radiusInKilometers": 15,
    "boundary": null
  },
  "workSchedule": {
    "timeZoneId": "Europe/Kyiv",
    "hours": [
      { "day": 1, "start": "09:00:00", "end": "18:00:00" }
    ]
  }
}
```

`day` має значення від 0 (неділя) до 6 (субота).

#### Response

`201 Created` з повним профілем:

```json
{
  "id": "0d3a5e8e-0c3b-4c5e-9a3f-2f6f1f0b9a11",
  "firstName": "First",
  "lastName": "Last",
  "avatar": null,
  "verification": "NotSubmitted",
  "verificationReview": null,
  "submittedAt": null,
  "verifiedAt": null,
  "revokedAt": null,
  "reverificationAllowedFrom": null,
  "verificationHold": null,
  "serviceArea": {
    "kind": "Radius",
    "center": { "latitude": 49.8397, "longitude": 24.0297 },
    "radiusInKilometers": 15,
    "boundary": []
  },
  "serviceAreaStatus": "Pending",
  "serviceAreaReview": null,
  "workSchedule": {
    "timeZoneId": "Europe/Kyiv",
    "hours": [{ "day": 1, "start": "09:00:00", "end": "18:00:00" }]
  },
  "isAvailable": false,
  "hasFeedAccess": false,
  "isRestricted": false,
  "isSuspended": false,
  "categories": [
    { "categoryId": "01a188c6-ef8c-7f93-b4f5-66d07a531083", "addedAt": "2026-11-09T14:00:00+00:00", "status": "Pending", "review": null }
  ],
  "documents": [],
  "sanctions": [],
  "createdAt": "2026-11-09T14:00:00+00:00",
  "lastUpdatedAt": null
}
```

### 1.2 GET /api/masters/me

**Доступ:** Worker

Повертає профіль поточного користувача у форматі [1.1](#11-post-apimasters).

### 1.3 PUT /api/masters/me/full-name

**Доступ:** Worker

Змінює імя і прізвище. Дозволено лише у статусах `NotSubmitted`, `Rejected`, `Revoked`, бо імя підтверджується документом особи.

#### Body

```json
{
  "firstName": "First",
  "lastName": "Last"
}
```

### 1.4 PUT /api/masters/me/service-area

**Доступ:** Worker

Змінює зону обслуговування. Змінена зона знову стає `Pending` і після верифікації потребує повторної перевірки.

#### Body

```json
{
  "center": null,
  "radiusInKilometers": null,
  "boundary": [
    { "latitude": 49.84, "longitude": 24.02 },
    { "latitude": 49.85, "longitude": 24.05 },
    { "latitude": 49.82, "longitude": 24.06 }
  ]
}
```

### 1.5 PUT /api/masters/me/work-schedule

**Доступ:** Worker

Змінює тижневий графік. Інтервали одного дня не можуть перетинатися. Графік не потребує перевірки.

#### Body

```json
{
  "timeZoneId": "Europe/Kyiv",
  "hours": [
    { "day": 1, "start": "09:00:00", "end": "13:00:00" },
    { "day": 1, "start": "14:00:00", "end": "18:00:00" }
  ]
}
```

### 1.6 PUT /api/masters/me/availability

**Доступ:** Worker

Вмикає або вимикає доступність. Разом із графіком і доступом до стрічки вона визначає, чи потрапляє майстер у кандидати Dispatch.

#### Body

```json
{
  "isAvailable": true
}
```

### 1.7 POST /api/masters/me/categories

**Доступ:** Worker

Додає категорію з каталогу. Архівовану категорію додати не можна. Нова категорія має статус `Pending`, а вже підтверджені категорії працюють і далі.

#### Body

```json
{
  "categoryId": "01a188c6-ef8c-7f93-b4f5-66d07a531083"
}
```

### 1.8 DELETE /api/masters/me/categories/{categoryId}

**Доступ:** Worker

Видаляє категорію з профілю. Профіль завжди має щонайменше одну категорію.

#### Path-параметри

| Параметр | Тип | Обов'язковий | Опис |
|--|--|--|--|
| `categoryId` | uuid | так | Ідентифікатор категорії |

### 1.9 PUT /api/masters/me/avatar

**Доступ:** Worker

Приймає фото профілю (`multipart/form-data`, поле `file`). Файл перевіряється асинхронно, див. [Завантаження файлів](#завантаження-файлів). Попереднє фото видаляється зі сховища після заміни.

#### Response

`202 Accepted`, заголовок `Location: /api/masters/me/uploads/{uploadId}`:

```json
{
  "id": "0193f1c2-4b7a-7c1e-9d2f-5a6b7c8d9e0f",
  "purpose": "Avatar",
  "categoryId": null,
  "fileName": "photo.jpg",
  "contentType": "image/jpeg",
  "sizeInBytes": 204800,
  "status": "Pending",
  "failureReason": null,
  "documentId": null,
  "createdAt": "2026-11-09T14:05:00+00:00",
  "processedAt": null
}
```

### 1.10 DELETE /api/masters/me/avatar

**Доступ:** Worker

Видаляє фото профілю. Файл зі сховища видаляється після фіксації зміни.

### 1.11 POST /api/masters/me/documents

**Доступ:** Worker

Приймає документ верифікації (`multipart/form-data`). Файл перевіряється асинхронно. Відповідь та сама, що в [1.9](#19-put-apimastersmeavatar).

#### Form-поля

| Поле | Тип | Обов'язкове | Опис |
|--|--|--|--|
| `file` | file | так | Документ: pdf, jpeg або png до 25 MiB |
| `kind` | string | так | `Identity` (документ особи) або `Qualification` (кваліфікація) |
| `categoryId` | uuid | для `Qualification` | Категорія профілю, яку підтверджує документ |

### 1.12 DELETE /api/masters/me/documents/{documentId}

**Доступ:** Worker

Видаляє документ. Під час верифікації (`Pending`) видаляти документи не можна.

### 1.13 GET /api/masters/me/documents/{documentId}

**Доступ:** Worker

Повертає вміст власного документа (файл із `Content-Disposition: attachment`).

### 1.14 GET /api/masters/me/uploads/{uploadId}

**Доступ:** Worker

Повертає статус завантаження у форматі відповіді [1.9](#19-put-apimastersmeavatar). Для `Failed` поле `failureReason` містить причину, наприклад невідповідність вмісту заявленому типу. Для прийнятого документа `documentId` вказує на документ профілю.

### 1.15 POST /api/masters/me/submit

**Доступ:** Worker

Надсилає профіль на верифікацію. Потрібні документ особи i документ кваліфікації для кожної категорії, яка ще не підтверджена. Після позбавлення верифікації повторна подача можлива лише після `reverificationAllowedFrom` і лише з документами, завантаженими після позбавлення.

### 1.16 GET /api/masters/{masterId}/avatar

**Доступ:** будь-який автентифікований користувач

Повертає фото профілю майстра (потік зображення). Якщо фото немає, повертає `404`.

## 2. Верифікація

### 2.1 GET /api/masters

**Доступ:** PlatformSupport, PlatformAdministrator

Черга профілів. Спершу йдуть профілі з найстарішою подачею.

#### Query-параметри

| Параметр | Тип | Обов'язковий | Опис |
|--|--|--|--|
| `verification` | string | ні | Фільтр за статусом верифікації, наприклад `Pending` |
| `pageNumber` | int | ні | Номер сторінки, від 1 (за замовчуванням 1) |
| `pageSize` | int | ні | Розмір сторінки, 1–100 (за замовчуванням 20) |

#### Response

```json
{
  "items": [
    {
      "id": "0d3a5e8e-0c3b-4c5e-9a3f-2f6f1f0b9a11",
      "firstName": "First",
      "lastName": "Lats",
      "verification": "Pending",
      "serviceAreaStatus": "Pending",
      "submittedAt": "2026-11-09T14:10:00+00:00",
      "isOnHold": false,
      "createdAt": "2026-11-09T14:00:00+00:00"
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 1,
  "totalPages": 1,
  "hasNextPage": false
}
```

### 2.2 GET /api/masters/{masterId}

**Доступ:** PlatformSupport, PlatformAdministrator

Повний профіль майстра у форматі [1.1](#11-post-apimasters).

### 2.3 GET /api/masters/{masterId}/documents/{documentId}

**Доступ:** PlatformSupport, PlatformAdministrator

Повертає вміст документа майстра для перевірки.

### 2.4 POST /api/masters/{masterId}/categories/{categoryId}/verify

**Доступ:** PlatformSupport, PlatformAdministrator

Підтверджує кваліфікацію в категорії. Потрібен документ кваліфікації. Під час блокування верифікації дія недоступна.

### 2.5 POST /api/masters/{masterId}/categories/{categoryId}/reject

**Доступ:** PlatformSupport, PlatformAdministrator

Відхиляє категорію з причиною, яку майстер бачить у сповіщенні.

#### Body

```json
{
  "reason": "Документ про кваліфікацію нечитабельний."
}
```

### 2.6 POST /api/masters/{masterId}/service-area/verify

**Доступ:** PlatformSupport, PlatformAdministrator

Підтверджує зону обслуговування.

### 2.7 POST /api/masters/{masterId}/service-area/reject

**Доступ:** PlatformSupport, PlatformAdministrator

Відхиляє зону обслуговування з причиною. Body такий самий, як у [2.5](#25-post-apimastersmasteridcategoriescategoryidreject).

### 2.8 POST /api/masters/{masterId}/verify

**Доступ:** PlatformSupport, PlatformAdministrator

Верифікує профіль. Умови: кожна категорія перевірена, хоча б одна підтверджена, зона підтверджена, блокування немає. Після цього майстер отримує доступ до стрічки.

### 2.9 POST /api/masters/{masterId}/reject

**Доступ:** PlatformSupport, PlatformAdministrator

Відхиляє профіль із причиною. Майстер може виправити профіль і подати його повторно. Body такий самий, як у [2.5](#25-post-apimastersmasteridcategoriescategoryidreject).

### 2.10 POST /api/masters/{masterId}/verification-hold

**Доступ:** PlatformSupport, PlatformAdministrator

Блокує верифікацію до перевірки людиною, наприклад через підозру на дублікат акаунта. Автоматичне блокування також застосовується за тимчасовим заходом Trust `DuplicateAccount`.

#### Body

```json
{
  "reason": "Збіг документа з іншим акаунтом."
}
```

### 2.11 DELETE /api/masters/{masterId}/verification-hold

**Доступ:** PlatformSupport, PlatformAdministrator

Знімає блокування верифікації після перевірки.

## 3. Каталог категорій

### 3.1 GET /api/categories

**Доступ:** Anonymous

Повертає каталог категорій, відсортований за назвою. Каталог є єдиним джерелом істини для Requests і Dispatch.

#### Query-параметри

| Параметр | Тип | Обов'язковий | Опис |
|--|--|--|--|
| `includeArchived` | bool | ні | Додати архівовані категорії; лише для `PlatformAdministrator`, інакше `403` |

#### Response

```json
[
  {
    "id": "01a188c6-ef8c-7f93-b4f5-66d07a531083",
    "name": "Сантехніка",
    "description": "Ремонт і встановлення сантехніки",
    "status": "Active",
    "createdAt": "2026-11-09T13:00:00+00:00",
    "lastUpdatedAt": null
  }
]
```

### 3.2 GET /api/categories/{categoryId}

**Доступ:** Anonymous

Повертає категорію у форматі елемента [3.1](#31-get-apicategories).

### 3.3 POST /api/categories

**Доступ:** PlatformAdministrator

Створює категорію. Назва унікальна серед категорій каталогу, повтор повертає `409`.

#### Body

```json
{
  "name": "Сантехніка",
  "description": "Ремонт і встановлення сантехніки"
}
```

#### Response

`201 Created` з категорією.

### 3.4 PUT /api/categories/{categoryId}

**Доступ:** PlatformAdministrator

Змінює назву та опис. Body такий самий, як у [3.3](#33-post-apicategories).

### 3.5 POST /api/categories/{categoryId}/archive

**Доступ:** PlatformAdministrator

Архівує категорію: її більше не можна обрати для нових профілів і заявок. Профілі, де вона вже є, не змінюються.

### 3.6 POST /api/categories/{categoryId}/activate

**Доступ:** PlatformAdministrator

Повертає архівовану категорію в каталог.