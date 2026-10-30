## Endpoints

Документація ендпоінтів модуля Identity.

### Обмеження частоти запитів

Кожен клієнт має спільний ліміт на весь API: 300 запитів за 60 секунд і не більше 10 запитів за секунду. Анонімного клієнта визначають за IP-адресою, автентифікованого за ідентифікатором користувача. Чутливі ендпоінти мають ще й власний ліміт, який рахується окремо для кожного ендпоінту. Коли ліміт перевищено, сервер повертає `429` із заголовком `Retry-After`.

| Ендпоінт | Власний ліміт |
|--|--|
| `register` | 5 за 60 с, не частіше 1 запиту за секунду |
| `login` | 10 за 60 с, не частіше 1 запиту за секунду (сплеск до 3) |
| `confirm-email` | 10 за 60 с |
| `resend-email-confirmation`, `forgot-password` | 3 за 60 с |
| `verify-mfa`, `reset-password` | 5 за 60 с |
| `refresh`, `logout` | 30 за 60 с |
| `users/me/verify-password`, `users/me/change-password`, `users/me/mfa` | 5 за 60 с |

## 1. Identity

### 1.1 POST /api/identity/register

**Доступ:** Anonymous

Реєструє нового користувача. Одразу після реєстрації на пошту надсилається лист для підтвердження. Поки пошту не підтверджено, увійти неможливо.

#### Body

```json
{
  "email": "user@example.com",
  "username": "user1",
  "password": "string"
}
```

#### Response

Ідентифікатор створеного користувача:

```json
"01a188c6-ef8c-7f93-b4f5-66d07a531083"
```

### 1.2 POST /api/identity/confirm-email

**Доступ:** Anonymous

Підтверджує, що пошта належить користувачу. Токен одноразовий і діє 24 години. Параметри `userId` і `token` фронтенд бере з посилання у листі.

#### Body

```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "token": "string"
}
```

### 1.3 POST /api/identity/resend-email-confirmation

**Доступ:** Anonymous

Повторно надсилає лист для підтвердження пошти, якщо користувач не встиг підтвердити її вчасно або попередній лист не дійшов.

Один акаунт отримує не більше одного листа на хвилину, з будь-якої IP-адреси. Повторний запит у межах цієї хвилини лист не надсилає, а відповідь лишається такою самою, як і для успішного запиту. Чинне лише посилання з останнього листа.

#### Body

```json
{
  "identifier": "string"
}
```

### 1.4 POST /api/identity/login

**Доступ:** Anonymous

Вхід за email або username. Якщо пошту не підтверджено, увійти неможливо. Після кількох невдалих спроб акаунт тимчасово блокується.

#### Body

```json
{
  "identifier": "string",
  "password": "string"
}
```

#### Response (MFA вимкнено)

```json
{
  "isMfaRequired": false,
  "tokens": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "accessTokenExpiresAt": "2026-10-29T18:08:06.8977055+00:00",
    "refreshToken": "xL6LQiKj2FDc0cr3V3kTcs6-pzWyHiRtuv5Xfh5Kwco",
    "refreshTokenExpiresAt": "2026-11-05T17:53:05.8226117+00:00"
  },
  "mfaChallenge": null
}
```

#### Response (MFA увімкнено)

```json
{
  "isMfaRequired": true,
  "tokens": null,
  "mfaChallenge": {
    "challengeToken": "_jvB44ov-8LQAlGR16UxQbJtCwSvxSdItlecYzv8RVA",
    "expiresAt": "2026-10-30T00:21:00.2243691+00:00"
  }
}
```


### 1.5 POST /api/identity/verify-mfa

**Доступ:** Anonymous

Завершує вхід, якщо в акаунті ввімкнено MFA. Під час логіну на пошту надходить лист із кодом, а `challengeToken` фронтенд бере з відповіді [1.4](#14-post-apiidentitylogin).

Правила:

- `challengeToken` і код діють 5 хвилин;
- на один `challengeToken` дається максимум 5 спроб;
- чинним є лише останній виклик: новий `login` скасовує попередній код;

#### Body

```json
{
  "challengeToken": "string",
  "code": "string"
}
```

#### Response

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "accessTokenExpiresAt": "2026-10-29T23:31:34.6747939+00:00",
  "refreshToken": "nrlj-Uiz4f_07to05wH9YbhvyFKlybrnqz3iuIhtKGA",
  "refreshTokenExpiresAt": "2026-11-05T23:16:34.3419385+00:00"
}
```

### 1.6 POST /api/identity/refresh

**Доступ:** Anonymous

Видає нову пару токенів без повторного входу. Access-token живе 15 хвилин, refresh-token живе 7 днів. При кожному оновленні видається нова пара, а попередній refresh-token перестає діяти. Якщо вже використаний refresh-token передати повторно, система вважає сесію скомпрометованою й відкликає її повністю.

#### Body

```json
{
  "refreshToken": "string"
}
```

#### Response

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "accessTokenExpiresAt": "2026-10-29T18:44:05.8880844+00:00",
  "refreshToken": "mR-8Htk9ObLvxw5lX0R9SndWZxmS5QaruTBq3hY4VbE",
  "refreshTokenExpiresAt": "2026-11-05T18:29:05.7468251+00:00"
}
```

#### Сценарій для фронтенду

1. Запит повертає `401` через протермінований access-token.
2. Фронтенд викликає `POST /api/identity/refresh` з refresh-token.
3. Зберігає нову пару токенів і повторює початковий запит.
4. Якщо refresh повернув `401`, редірект на сторінку входу.

### 1.7 POST /api/identity/logout

**Доступ:** Anonymous (сесія визначається за refresh-token)

Завершує поточну сесію: refresh-token, який передається, відкликається.

#### Body

```json
{
  "refreshToken": "string"
}
```

### 1.8 POST /api/identity/logout-all

**Доступ:** Bearer

Завершує всі активні сесії користувача, включно з поточною, і відкликає всі refresh-токени. Тіло запиту не потрібне.

### 1.9 POST /api/identity/forgot-password

**Доступ:** Anonymous

Надсилає на пошту посилання для скидання пароля. Скинути пароль можна протягом 1 години.

Один акаунт отримує не більше одного листа на хвилину, з будь-якої IP-адреси. Повторний запит у межах цієї хвилини лист не надсилає. Відповідь завжди однакова, тож за нею не можна визначити, чи існує акаунт. Чинне лише посилання з останнього листа.

#### Body

```json
{
  "identifier": "string"
}
```

### 1.10 POST /api/identity/reset-password

**Доступ:** Anonymous

Встановлює новий пароль за токеном із листа. Параметри `userId` і `token` фронтенд бере з посилання. Наслідки: відкликаються всі активні сесії; тимчасове блокування акаунта знімається.

Новий пароль має відрізнятися від поточного, інакше сервер повертає `400`. Токен при цьому вже використано, тому користувачу треба запросити новий лист через [1.9](#19-post-apiidentityforgot-password).

#### Body

```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "token": "string",
  "newPassword": "string"
}
```

## 2. Users

Ендпоінти з доступом `PlatformAdministrator` ([2.7](#27-put-apiusersuseridroles)–[2.9](#29-post-apiusersuseridactivate)) повертають `401` без токена і `403`, якщо в токені немає ролі `PlatformAdministrator`. Роль береться з access-token, тому після зміни ролей вона набуває чинності з наступним `refresh` (не пізніше ніж через 15 хвилин).

### 2.1 GET /api/users/me

**Доступ:** Bearer

Повертає інформацію про поточного користувача.

### Response

```json
{
  "id": "01a188c6-ef8c-7f93-b4f5-66d07a531083",
  "email": "user@example.com",
  "username": "user1",
  "emailConfirmed": true,
  "isActive": true,
  "mfaEnabled": false,
  "roles": [3],
  "createdAt": "2026-10-29T17:48:21.259952+00:00",
  "lastLoginAt": "2026-10-29T23:23:22.377124+00:00"
}
```

### 2.2 POST /api/users/me/verify-password

**Доступ:** Bearer

Підтверджує, що користувач знає поточний пароль. Використовується як повторна автентифікація перед чутливими операціями.

#### Body

```json
{
  "password": "string"
}
```

### 2.3 POST /api/users/me/change-password

**Доступ:** Bearer

Змінює пароль користувача. Усі інші сесії, крім поточної, відкликаються. Новий пароль має відрізнятися від поточного, інакше сервер повертає `400`.

### Body

```json
{
  "currentPassword": "string",
  "newPassword": "string"
}
```

### 2.4 PUT /api/users/me/mfa

**Доступ:** Bearer

Вмикає або вимикає MFA. Для підтвердження потрібен поточний пароль. Увімкнути MFA можна лише після підтвердження пошти, бо код надходить листом. Повторний виклик із тим самим значенням `enabled` нічого не змінює.

```json
{
  "password": "string",
  "enabled": true
}
```

### 2.5 GET /api/users/me/sessions

**Доступ:** Bearer

Повертає всі активні сесії поточного користувача.

### Response

```json
[
  {
    "sessionId": "01a18a18-0a2f-7b7a-ac3b-9caccfc2001a",
    "createdAt": "2026-10-29T23:56:33.265706+00:00",
    "lastActivityAt": "2026-10-29T23:56:33.265706+00:00",
    "expiresAt": "2026-11-28T23:56:33.265706+00:00",
    "mfaVerified": false,
    "ipAddress": "::1",
    "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ...",
    "deviceId": null,
    "isCurrent": true
  }
]
```

### 2.6 DELETE /api/users/me/sessions/{sessionId}

**Доступ:** Bearer

Відкликає сесію користувача за вказаним ідентифікатором.

#### Path-параметри

| Параметр | Тип | Обов'язковий | Опис|
|--|--|--|--|
| `sessionId` | uuid | так | Ідентифікатор сесії |

### 2.7 PUT /api/users/{userId}/roles

**Доступ:** PlatformAdministrator

Оновлює ролі користувача за вказаним ідентифікатором. Ролі передаються індексами. Адміністратор не може позбавити себе ролі адміністратора.

#### Path-параметри

| Параметр | Тип  | Обов'язковий | Опис |
|--|--|--|--|
| `userId` | uuid | так | Ідентифікатор користувача |

#### Body

```json
{
  "roles": [3]
}
```

### 2.8 POST /api/users/{userId}/deactivate

**Доступ:** PlatformAdministrator

Деактивує акаунт користувача за вказаним ідентифікатором. Деактивований акаунт не може увійти в систему.

#### Path-параметри

| Параметр | Тип | Обов'язковий | Опис |
|--|--|--|--|
| `userId` | uuid | так | Ідентифікатор користувача |

### 2.9 POST /api/users/{userId}/activate

**Доступ:** PlatformAdministrator

Активує акаунт користувача за вказаним ідентифікатором.

#### Path-параметри

| Параметр | Тип | Обов'язковий | Опис |
|--|--|--|--|
| `userId` | uuid | так | Ідентифікатор користувача |