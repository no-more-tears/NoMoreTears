---
title: Identity Provider Flows
document-type: Design Document
system: NoMoreTears
scope: Shared/Providers/Identity
related-docs:
    - architecture
    - domain-driven-design.example
tags: [ identity, authentication, mfa, sessions, jwt, refresh-tokens, flows, mermaid ]
created-at: 16-10-2026
last-updated-at: 16-10-2026
---

## Потоки Identity Provider

Основні потоки shared-модуля `src/backend/Shared/Providers/Identity` у вигляді діаграм Mermaid. Джерело істини: `IdentityProvider` (partial-файли за можливостями), `MfaChallengeService`, `UserActionTokenService`, `JwtBearerOptionsConfigurator`, `IdentityMaintenanceService`. Модуль не надсилає листів і кодів: він повертає тікет або код хосту.

### 0. Умовні позначення

| Клас | Заливка / обвідка | Форма | Значення |
|--|--|--|--|
| `core` | `#0D1117` / `#FFFFFF` | заокруглений прямокутник | вхід у потік, успішний результат, ключовий компонент |
| `support` | `#1C2128` / `#A5A5A5` | прямокутник, циліндр | крок модуля; циліндр — запис або умовний `UPDATE` у БД |
| `generic` | `#2D333B` / `#6E7681` | ромб | перевірка або розгалуження; на огляді — допоміжний сервіс |
| `fail` | `#2D333B` / пунктир `#6E7681` | прямокутник зі зрізом | відмова: виняток, який отримує хост |
| `ext` | `#1C2128` / пунктир `#A5A5A5` | прямокутник | дія поза модулем: хост, клієнт, пошта |

Доступність діаграм:
- кожна діаграма має `accTitle` і `accDescr` для читачів екрана;
- значення передається формою вузла та підписом ребра («так» / «ні»), а не лише кольором;
- увесь текст лежить на темній заливці з контрастом понад 12:1, тому діаграми читаються і в темній, і у світлій темі.

### 1. Огляд компонентів

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart LR
    accTitle: Огляд компонентів Identity Provider
    accDescr: Хост викликає сегреговані інтерфейси фасаду IIdentityProvider. Їх реалізує IdentityProvider, який користується хешером паролів, провайдером JWT, сервісами MFA-челенджів і одноразових токенів та зберігає дані через IdentityDbContext у схемі identity PostgreSQL. Bearer-хук хоста перевіряє активність сесії, фоновий worker прибирає сховище. Доставка листів і кодів лишається на хості.

    Host["Хост<br/>API або IdentityService"]
    Mail["Доставка email або коду<br/>поза модулем"]

    subgraph FACADE["IIdentityProvider"]
        SignUp["ISignUpService"]
        SignIn["ISignInService"]
        Account["IAccountService"]
        Sessions["ISessionService"]
        Admin["IUserAdministrationService"]
    end

    Provider["IdentityProvider<br/>partial: SignUp · SignIn · Account · Sessions · Administration"]

    subgraph SERVICES["Внутрішні сервіси"]
        Hasher["IPasswordHasher<br/>BCrypt enhanced + DummyPasswordHash"]
        Jwt["IJwtProvider<br/>HS256, typ at+jwt"]
        Mfa["IMfaChallengeService"]
        Action["IUserActionTokenService"]
    end

    Db[("IdentityDbContext<br/>схема identity, snake_case")]
    Bearer["JwtBearerOptionsConfigurator<br/>OnTokenValidated → сесія sid активна?"]
    Worker["IdentityCleanupWorker<br/>→ IIdentityMaintenanceService"]

    Host --> SignUp & SignIn & Account & Sessions & Admin --> Provider
    Provider --> Hasher & Jwt & Mfa & Action
    Provider --> Db
    Mfa --> Db
    Action --> Db
    Host -.->|"тікет або код"| Mail
    Bearer --> Sessions
    Worker --> Db

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef ext fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:1px,stroke-dasharray: 5 5;

    class Provider core;
    class SignUp,SignIn,Account,Sessions,Admin,Db,Bearer,Worker support;
    class Hasher,Jwt,Mfa,Action generic;
    class Host,Mail ext;

    style FACADE fill:#0D1117,stroke:#FFFFFF,stroke-width:1px,stroke-dasharray: 5 5
    style SERVICES fill:#0D1117,stroke:#6E7681,stroke-width:1px,stroke-dasharray: 5 5

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 2. Модель даних

Усі секрети зберігаються лише як SHA-256 дайджести. Видалення користувача каскадно видаляє все; видалення сесії каскадно видаляє її refresh-токени.

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#0D1117","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","textColor":"#FFFFFF","nodeTextColor":"#FFFFFF","mainBkg":"#0D1117","nodeBorder":"#A5A5A5","lineColor":"#B1BAC4","tertiaryColor":"#1C2128","attributeBackgroundColorOdd":"#1C2128","attributeBackgroundColorEven":"#2D333B"}}}%%
erDiagram
    accTitle: Модель даних схеми identity
    accDescr: Таблиця users має ролі user_roles, сесії sessions, MFA-челенджі mfa_challenges та одноразові токени дій user_action_tokens. Сесія володіє ланцюжком ротованих refresh_tokens, кожен токен знає свого наступника через replaced_by_token_id. Унікальні індекси лежать на normalized_email, normalized_username та на хешах токенів.

    users ||--o{ user_roles : "ролі"
    users ||--o{ sessions : "відкриває"
    users ||--o{ mfa_challenges : "проходить"
    users ||--o{ user_action_tokens : "отримує"
    sessions ||--o{ refresh_tokens : "ротує"

    users {
        uuid id PK
        varchar email
        varchar normalized_email UK
        varchar username
        varchar normalized_username UK
        varchar password_hash
        boolean email_confirmed
        boolean is_active
        boolean mfa_enabled
        integer failed_access_attempts
        timestamptz lockout_end_at
        timestamptz created_at
        timestamptz password_changed_at
        timestamptz last_login_at
    }

    user_roles {
        uuid user_id PK, FK
        varchar role PK
    }

    sessions {
        uuid id PK
        uuid user_id FK
        timestamptz created_at
        timestamptz last_activity_at
        timestamptz expires_at
        boolean mfa_verified
        timestamptz revoked_at
        varchar revocation_reason
        varchar ip_address
        varchar user_agent
        varchar device_id
    }

    refresh_tokens {
        uuid id PK
        uuid session_id FK
        varchar token_hash UK
        timestamptz created_at
        timestamptz expires_at
        timestamptz consumed_at
        uuid replaced_by_token_id
    }

    mfa_challenges {
        uuid id PK
        uuid user_id FK
        varchar challenge_hash UK
        varchar code_hash
        varchar client_fingerprint
        timestamptz created_at
        timestamptz expires_at
        timestamptz consumed_at
        integer failed_attempts
    }

    user_action_tokens {
        uuid id PK
        uuid user_id FK
        varchar purpose
        varchar token_hash UK
        timestamptz created_at
        timestamptz expires_at
        timestamptz consumed_at
    }
```

### 3. Реєстрація

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart TD
    accTitle: Реєстрація користувача
    accDescr: RegisterAsync перевіряє політику пароля, нормалізує email і username, перевіряє унікальність обох ідентифікаторів за обома колонками, створює користувача з роллю і хешем BCrypt, перехоплює гонку унікальності як ConflictException, видає одноразовий тікет підтвердження email і повертає RegistrationResult. Лист надсилає хост.

    Start(["RegisterRequest<br/>Email, Username?, Password, Role"]) --> Policy{"Довжина пароля в межах<br/>MinimumLength … MaximumLength?"}
    Policy -->|"ні"| BadReq>"BadRequestException<br/>PasswordTooShort або PasswordTooLong"]
    Policy -->|"так"| Norm["Trim + ToUpperInvariant<br/>Username ← Email, якщо порожній"]
    Norm --> Taken{"normalized_email або normalized_username<br/>уже зайняті в будь-якій з двох колонок?"}
    Taken -->|"так"| Conflict>"ConflictException<br/>DuplicateUser"]
    Taken -->|"ні"| Create[("INSERT users + user_roles<br/>password_hash = BCrypt, password_changed_at = now")]
    Create --> Unique{"Порушення унікальності<br/>під час SaveChanges?"}
    Unique -->|"так"| Conflict
    Unique -->|"ні"| Ticket[("IssueAsync EmailConfirmation<br/>попередні токени → consumed<br/>новий token_hash = SHA-256, TTL 24 год")]
    Ticket --> Result(["RegistrationResult<br/>UserId, EmailConfirmation, RequiresEmailConfirmation"])
    Result --> Host["Хост надсилає лист із токеном"]

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef fail fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px,stroke-dasharray: 5 5;
    classDef ext fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:1px,stroke-dasharray: 5 5;

    class Start,Result core;
    class Norm,Create,Ticket support;
    class Policy,Taken,Unique generic;
    class BadReq,Conflict fail;
    class Host ext;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 4. Підтвердження email

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart TD
    accTitle: Підтвердження email і повторне надсилання
    accDescr: ConfirmEmailAsync знаходить користувача за UserId, ідемпотентно завершується, якщо email уже підтверджено, інакше умовним UPDATE споживає токен EmailConfirmation і виставляє email_confirmed. RequestEmailConfirmationAsync повертає новий тікет лише активному користувачу з непідтвердженим email, інакше null без розкриття існування акаунта.

    C0(["ConfirmEmailRequest<br/>UserId, Token"]) --> C1{"Користувач існує?"}
    C1 -->|"ні"| CF>"UnauthorizedException<br/>InvalidToken"]
    C1 -->|"так"| C2{"email_confirmed уже true?"}
    C2 -->|"так"| C3(["Без змін<br/>ідемпотентно"])
    C2 -->|"ні"| C4[("ConsumeAsync: UPDATE user_action_tokens<br/>SET consumed_at = now<br/>WHERE user_id, purpose, token_hash,<br/>consumed_at IS NULL, expires_at > now")]
    C4 --> C5{"Оновлено рівно 1 рядок?"}
    C5 -->|"ні"| CF
    C5 -->|"так"| C6[("users.email_confirmed = true")]
    C6 --> C7(["Email підтверджено"])

    R0(["RequestEmailConfirmationAsync<br/>identifier"]) --> R1{"Користувач активний<br/>і email не підтверджено?"}
    R1 -->|"ні"| R2(["null<br/>хост відповідає однаково"])
    R1 -->|"так"| R3[("IssueAsync EmailConfirmation<br/>новий тікет, попередні → consumed")]
    R3 --> R4(["UserActionTicket<br/>UserId, Email, Purpose, Token, ExpiresAt"])
    R4 --> R5["Хост надсилає лист"]

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef fail fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px,stroke-dasharray: 5 5;
    classDef ext fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:1px,stroke-dasharray: 5 5;

    class C0,C3,C7,R0,R2,R4 core;
    class C4,C6,R3 support;
    class C1,C2,C5,R1 generic;
    class CF fail;
    class R5 ext;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 5. Вхід за паролем

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart TD
    accTitle: Вхід за паролем
    accDescr: LoginAsync шукає користувача за email або username і завжди перевіряє пароль, для невідомого користувача проти фіктивного хешу, щоб час відповіді не розкривав існування акаунта. Далі перевіряє lockout, результат пароля, активність і підтвердження email, за потреби перехешовує пароль, скидає лічильник невдач і або створює MFA-челендж, або відкриває сесію та видає пару токенів.

    L0(["LoginRequest<br/>Identifier, Password, Client"]) --> L1["FindUserAsync<br/>normalized_email або normalized_username"]
    L1 --> L2["VerifyPassword<br/>hash користувача або DummyPasswordHash<br/>однаковий час відповіді"]
    L2 --> L3{"Користувач існує?"}
    L3 -->|"ні"| LU>"UnauthorizedException<br/>InvalidCredentials"]
    L3 -->|"так"| L4{"lockout_end_at > now?"}
    L4 -->|"так"| LL>"TooManyRequestsException<br/>AccountLocked"]
    L4 -->|"ні"| L5{"Пароль вірний?"}
    L5 -->|"ні"| L6[("RegisterFailedAccessAsync<br/>failed_access_attempts + 1<br/>≥ MaxFailedAccessAttempts → lockout_end_at = now + LockoutDuration, лічильник = 0")]
    L6 --> LU
    L5 -->|"так"| L7{"is_active?"}
    L7 -->|"ні"| LD>"ForbiddenException<br/>AccountDeactivated"]
    L7 -->|"так"| L8{"RequireConfirmedEmail<br/>і email не підтверджено?"}
    L8 -->|"так"| LE>"ForbiddenException<br/>EmailNotConfirmed"]
    L8 -->|"ні"| L9{"Потрібен rehash<br/>хеш зі старим WorkFactor?"}
    L9 -->|"так"| L10["password_hash ← BCrypt з поточним WorkFactor"]
    L9 -->|"ні"| L11
    L10 --> L11["failed_access_attempts = 0<br/>lockout_end_at = null"]
    L11 --> L12{"mfa_enabled?"}
    L12 -->|"так"| L13[("SaveChanges<br/>IMfaChallengeService.CreateAsync")]
    L13 --> L14(["LoginResult.MfaRequired<br/>MfaChallenge: ChallengeToken, Code, ExpiresAt"])
    L14 --> L15["Хост доставляє Code користувачу,<br/>клієнту віддає лише ChallengeToken"]
    L12 -->|"ні"| L16[("last_login_at = now<br/>StartSessionAsync, mfaVerified = false")]
    L16 --> L17(["LoginResult.Authenticated<br/>TokenPair"])

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef fail fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px,stroke-dasharray: 5 5;
    classDef ext fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:1px,stroke-dasharray: 5 5;

    class L0,L14,L17 core;
    class L1,L2,L6,L10,L11,L13,L16 support;
    class L3,L4,L5,L7,L8,L9,L12 generic;
    class LU,LL,LD,LE fail;
    class L15 ext;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 6. MFA-челендж

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart TD
    accTitle: Створення та перевірка MFA-челенджу
    accDescr: CreateAsync закриває попередні відкриті челенджі користувача і зберігає хеш токена челенджу, хеш коду, прив'язаний до токена, та відбиток клієнта з User-Agent і DeviceId. VerifyMfaAsync через VerifyAsync відхиляє невідомий, спожитий, прострочений, вичерпаний за спробами або чужий за клієнтом челендж, лічить невірні коди, споживає челендж умовним UPDATE рівно одним викликом і відкриває сесію з ознакою MfaVerified.

    M0(["CreateAsync<br/>userId, client"]) --> M1[("UPDATE mfa_challenges SET consumed_at = now<br/>WHERE user_id, consumed_at IS NULL<br/>один відкритий челендж на користувача")]
    M1 --> M2["challengeToken = 32 випадкових байти, base64url<br/>code = CodeLength цифр"]
    M2 --> M3[("INSERT mfa_challenges<br/>challenge_hash = SHA-256 від token<br/>code_hash = SHA-256 від token + ':' + code<br/>client_fingerprint = SHA-256 від UserAgent + DeviceId<br/>expires_at = now + ChallengeLifetime")]
    M3 --> M4(["MfaChallenge<br/>ChallengeToken, Code, ExpiresAt"])
    M4 --> M5["Хост доставляє Code,<br/>клієнт отримує лише ChallengeToken"]

    V0(["VerifyMfaRequest<br/>ChallengeToken, Code, Client"]) --> V1["Знайти челендж за SHA-256 від ChallengeToken"]
    V1 --> V2{"Існує, не спожитий, не прострочений,<br/>failed_attempts < MaxAttempts,<br/>відбиток клієнта збігається?"}
    V2 -->|"ні"| VF>"UnauthorizedException<br/>InvalidMfaCode"]
    V2 -->|"так"| V3{"code_hash збігається?<br/>порівняння за сталий час"}
    V3 -->|"ні"| V4[("failed_attempts + 1")]
    V4 --> VF
    V3 -->|"так"| V5[("UPDATE SET consumed_at = now<br/>WHERE id, consumed_at IS NULL,<br/>failed_attempts < MaxAttempts")]
    V5 --> V6{"Оновлено рівно 1 рядок?"}
    V6 -->|"ні"| VF
    V6 -->|"так"| V7{"Користувач існує, активний,<br/>email підтверджено за політикою?"}
    V7 -->|"ні"| VD>"UnauthorizedException або<br/>ForbiddenException"]
    V7 -->|"так"| V8[("last_login_at = now<br/>StartSessionAsync, mfaVerified = true")]
    V8 --> V9(["TokenPair<br/>amr = pwd + otp"])

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef fail fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px,stroke-dasharray: 5 5;
    classDef ext fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:1px,stroke-dasharray: 5 5;

    class M0,M4,V0,V9 core;
    class M1,M2,M3,V1,V4,V5,V8 support;
    class V2,V3,V6,V7 generic;
    class VF,VD fail;
    class M5 ext;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 7. Відкриття сесії та склад токенів

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart LR
    accTitle: Відкриття сесії та склад токенів
    accDescr: StartSessionAsync створює сесію з абсолютним терміном життя та метаданими клієнта і перший refresh-токен зі ковзним терміном, що не перевищує термін сесії, зберігає обидва і видає пару токенів. Access-токен є JWT HS256 типу at+jwt із claim-ами sub, email, name, sid, amr, role та jti. Refresh-токен є непрозорим секретом, у БД зберігається лише його SHA-256.

    S0(["StartSessionAsync<br/>user, client, mfaVerified"]) --> S1[("sessions<br/>id = UUIDv7<br/>expires_at = now + SessionLifetime<br/>mfa_verified, ip_address, user_agent, device_id")]
    S0 --> S2[("refresh_tokens<br/>token_hash = SHA-256 від secret<br/>expires_at = min від now + RefreshTokenLifetime<br/>та session.expires_at")]
    S1 --> S3["IssueTokens"]
    S2 --> S3
    S3 --> S4["Access JWT, HS256, typ at+jwt<br/>TTL = AccessTokenLifetime<br/>sub, email, name, sid, jti<br/>amr = pwd, за MFA також otp<br/>role, по одному на роль"]
    S3 --> S5["Refresh secret<br/>base64url, 32 байти<br/>повертається один раз"]
    S4 --> S6(["TokenPair<br/>AccessToken, AccessTokenExpiresAt,<br/>RefreshToken, RefreshTokenExpiresAt, SessionId"])
    S5 --> S6

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;

    class S0,S6 core;
    class S1,S2,S3 support;
    class S4,S5 generic;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 8. Оновлення токенів

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart TD
    accTitle: Оновлення пари токенів із ротацією refresh-токена
    accDescr: RefreshAsync шукає refresh-токен за SHA-256, трактує вже спожитий токен як повторне використання і відкликає сесію, відхиляє відкликану чи прострочену сесію, прострочений токен і неактивного користувача, створює наступний токен, умовним UPDATE споживає поточний так, що з конкурентних викликів перемагає рівно один, оновлює активність сесії та IP і видає нову пару токенів.

    F0(["RefreshRequest<br/>RefreshToken, Client"]) --> F1["Знайти refresh_tokens за SHA-256 від RefreshToken<br/>разом із сесією"]
    F1 --> F2{"Токен знайдено?"}
    F2 -->|"ні"| FU>"UnauthorizedException<br/>InvalidRefreshToken"]
    F2 -->|"так"| F3{"consumed_at уже встановлено?"}
    F3 -->|"так"| FR[("HandleReuseAsync<br/>сесія → revoked_at = now, ReuseDetected")]
    FR --> FU
    F3 -->|"ні"| F4{"Сесія не відкликана,<br/>сесія і токен не прострочені?"}
    F4 -->|"ні"| FU
    F4 -->|"так"| F5{"Користувач існує і активний?"}
    F5 -->|"ні"| FU
    F5 -->|"так"| F6["CreateRefreshToken<br/>новий secret, expires_at = min від ковзного та сесії"]
    F6 --> F7[("UPDATE refresh_tokens<br/>SET consumed_at = now, replaced_by_token_id = next.id<br/>WHERE id = current.id AND consumed_at IS NULL")]
    F7 --> F8{"Оновлено рівно 1 рядок?<br/>конкурентний refresh програє"}
    F8 -->|"ні"| FR
    F8 -->|"так"| F9[("sessions.last_activity_at = now<br/>ip_address ← Client.IpAddress або попередній<br/>INSERT наступний refresh_token")]
    F9 --> F10(["TokenPair<br/>новий access JWT + новий refresh secret"])

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef fail fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px,stroke-dasharray: 5 5;

    class F0,F10 core;
    class F1,F6,F7,F9,FR support;
    class F2,F3,F4,F5,F8 generic;
    class FU fail;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 9. Відкликання сесій

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart LR
    accTitle: Джерела відкликання сесій та їх наслідки
    accDescr: Сесію відкликають вихід за refresh-токеном, вихід з усіх сесій, відкликання з переліку сесій, зміна пароля зі збереженням поточної сесії, скидання пароля, деактивація акаунта та виявлення повторного використання refresh-токена. Відкликання є умовним UPDATE полів revoked_at і revocation_reason. Після цього bearer-хук відхиляє access-токени сесії, RefreshAsync відхиляє її refresh-токени, сесія зникає з переліку, а фонове прибирання видаляє її після періоду зберігання.

    A1["LogoutAsync<br/>refreshToken → sessionId"] -->|"Logout"| R
    A2["LogoutAllAsync<br/>userId"] -->|"LogoutAll, усі сесії"| R
    A3["RevokeSessionAsync<br/>userId + sessionId, лише власна"] -->|"RevokedByUser"| R
    A4["ChangePasswordAsync"] -->|"PasswordChanged,<br/>крім CurrentSessionId"| R
    A5["ResetPasswordAsync"] -->|"PasswordReset, усі сесії"| R
    A6["DeactivateAsync"] -->|"AccountDeactivated, усі сесії"| R
    A7["RefreshAsync<br/>повтор спожитого токена"] -->|"ReuseDetected"| R

    R[("UPDATE sessions<br/>SET revoked_at = now, revocation_reason = …<br/>WHERE revoked_at IS NULL")]
    R --> E1["Bearer-хук: IsSessionActiveAsync = false<br/>access-токен відхилено"]
    R --> E2["RefreshAsync: сесія відкликана<br/>InvalidRefreshToken"]
    R --> E3["GetSessionsAsync: сесії немає в переліку"]
    R --> E4["Прибирання: DELETE після RevokedSessionRetention"]

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;

    class R core;
    class A1,A2,A3,A4,A5,A6,A7 support;
    class E1,E2,E3,E4 generic;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 10. Перевірка access-токена на боці хоста

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart TD
    accTitle: Перевірка access-токена bearer-схемою хоста
    accDescr: UseJwtBearer налаштовує JwtBearerOptions тими самими JwtOptions: перевірка підпису HS256, issuer, audience, типу at+jwt, терміну дії з ClockSkew, без inbound claim mapping. Після стандартної валідації хук OnTokenValidated читає claim sid і відхиляє токен, якщо сесію відкликано або вона прострочена, якщо тільки validateSessions не вимкнено.

    B0(["HTTP-запит<br/>Authorization: Bearer JWT"]) --> B1{"Підпис HS256, iss, aud,<br/>typ = at+jwt, exp з ClockSkew<br/>валідні?"}
    B1 -->|"ні"| BF>"401<br/>токен відхилено"]
    B1 -->|"так"| B2{"validateSessions = true?"}
    B2 -->|"ні"| B5
    B2 -->|"так"| B3{"Claim sid є і це GUID?"}
    B3 -->|"ні"| BS>"context.Fail<br/>The token does not carry a session identifier"]
    B3 -->|"так"| B4[("ISessionService.IsSessionActiveAsync<br/>sessions WHERE id = sid<br/>AND revoked_at IS NULL AND expires_at > now")]
    B4 --> B6{"Сесія активна?"}
    B6 -->|"ні"| BR>"context.Fail<br/>The session has been revoked"]
    B6 -->|"так"| B5(["ClaimsPrincipal автентифіковано<br/>GetUserId, GetSessionId, GetRoles, HasVerifiedMfa"])

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef fail fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px,stroke-dasharray: 5 5;

    class B0,B5 core;
    class B4 support;
    class B1,B2,B3,B6 generic;
    class BF,BS,BR fail;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 11. Зміна пароля

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart TD
    accTitle: Зміна пароля автентифікованим користувачем
    accDescr: ChangePasswordAsync перевіряє політику нового пароля, вимагає вірний поточний пароль, зберігає новий хеш BCrypt і момент зміни, після чого відкликає всі сесії користувача, крім поточної.

    P0(["ChangePasswordRequest<br/>UserId, CurrentPassword, NewPassword, CurrentSessionId"]) --> P1{"Новий пароль у межах політики?"}
    P1 -->|"ні"| PB>"BadRequestException"]
    P1 -->|"так"| P2{"Користувач існує і<br/>CurrentPassword вірний?"}
    P2 -->|"ні"| PU>"UnauthorizedException<br/>InvalidCredentials"]
    P2 -->|"так"| P3[("password_hash ← BCrypt від NewPassword<br/>password_changed_at = now")]
    P3 --> P4[("Відкликати сесії: PasswordChanged<br/>крім CurrentSessionId")]
    P4 --> P5(["Пароль змінено,<br/>поточна сесія жива"])

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef fail fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px,stroke-dasharray: 5 5;

    class P0,P5 core;
    class P3,P4 support;
    class P1,P2 generic;
    class PB,PU fail;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 12. Відновлення пароля

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart TD
    accTitle: Забутий пароль і скидання пароля
    accDescr: ForgotPasswordAsync повертає тікет PasswordReset лише активному користувачу, інакше null без розкриття існування акаунта. ResetPasswordAsync перевіряє політику пароля, спочатку споживає токен умовним UPDATE, потім зберігає новий хеш, підтверджує email, знімає lockout і відкликає всі сесії.

    G0(["ForgotPasswordAsync<br/>identifier"]) --> G1{"Користувач існує і активний?"}
    G1 -->|"ні"| G2(["null<br/>хост відповідає однаково"])
    G1 -->|"так"| G3[("IssueAsync PasswordReset<br/>TTL 1 год, попередні → consumed")]
    G3 --> G4(["UserActionTicket"])
    G4 --> G5["Хост надсилає лист"]

    T0(["ResetPasswordRequest<br/>UserId, Token, NewPassword"]) --> T1{"Новий пароль у межах політики?"}
    T1 -->|"ні"| TB>"BadRequestException"]
    T1 -->|"так"| T2[("ConsumeAsync PasswordReset<br/>умовний UPDATE consumed_at")]
    T2 --> T3{"Оновлено рівно 1 рядок<br/>і користувач існує?"}
    T3 -->|"ні"| TU>"UnauthorizedException<br/>InvalidToken"]
    T3 -->|"так"| T4[("password_hash ← BCrypt від NewPassword<br/>password_changed_at = now<br/>email_confirmed = true<br/>failed_access_attempts = 0, lockout_end_at = null")]
    T4 --> T5[("Відкликати всі сесії: PasswordReset")]
    T5 --> T6(["Пароль скинуто,<br/>усі сесії завершено"])

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef fail fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px,stroke-dasharray: 5 5;
    classDef ext fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:1px,stroke-dasharray: 5 5;

    class G0,G2,G4,T0,T6 core;
    class G3,T2,T4,T5 support;
    class G1,T1,T3 generic;
    class TB,TU fail;
    class G5 ext;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 13. Керування MFA та адміністрування

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart TD
    accTitle: Увімкнення MFA та адміністрування акаунта
    accDescr: UpdateMfaAsync вимагає пароль, дозволяє вмикати MFA лише з підтвердженим email і є ідемпотентним. UpdateRolesAsync синхронізує набір ролей. DeactivateAsync вимикає акаунт і відкликає всі сесії. ReactivateAsync вмикає акаунт і знімає lockout. Невідомий користувач для адмін-операцій дає NotFoundException.

    U0(["UpdateMfaRequest<br/>UserId, Password, Enabled"]) --> U1{"Користувач існує і пароль вірний?"}
    U1 -->|"ні"| UU>"UnauthorizedException<br/>InvalidCredentials"]
    U1 -->|"так"| U2{"Enabled і email не підтверджено?"}
    U2 -->|"так"| UF>"ForbiddenException<br/>MfaRequiresConfirmedEmail"]
    U2 -->|"ні"| U3{"mfa_enabled уже дорівнює Enabled?"}
    U3 -->|"так"| U4(["Без змін"])
    U3 -->|"ні"| U5[("mfa_enabled = Enabled")]
    U5 --> U6(["MFA оновлено"])

    D0(["Адмін-операція<br/>userId"]) --> D1{"Користувач існує?"}
    D1 -->|"ні"| DN>"NotFoundException<br/>UserNotFound"]
    D1 -->|"так"| D2[("UpdateRolesAsync<br/>видалити зайві, додати відсутні ролі")]
    D1 -->|"так"| D3[("DeactivateAsync<br/>is_active = false")]
    D3 --> D4[("Відкликати всі сесії<br/>AccountDeactivated")]
    D1 -->|"так"| D5[("ReactivateAsync<br/>is_active = true, lockout знято")]

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef fail fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px,stroke-dasharray: 5 5;

    class U0,U4,U6,D0 core;
    class U5,D2,D3,D4,D5 support;
    class U1,U2,U3,D1 generic;
    class UU,UF,DN fail;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 14. Фонове прибирання

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart TD
    accTitle: Фонове прибирання сховища
    accDescr: IdentityCleanupWorker за PeriodicTimer з інтервалом MaintenanceOptions.Interval у новій DI-області викликає PurgeExpiredAsync. Той пакетно видаляє прострочені чи спожиті токени дій і MFA-челенджі, сесії, що прострочені або відкликані довше за RevokedSessionRetention, разом із їхніми refresh-токенами каскадом, і refresh-токени, прострочені довше за той самий період. Помилка одного запуску логується і не зупиняє хост.

    W0(["UseBackgroundCleanup<br/>interval?"]) --> W1["PeriodicTimer<br/>MaintenanceOptions.Interval"]
    W1 --> W2["CreateAsyncScope<br/>IIdentityMaintenanceService.PurgeExpiredAsync"]
    W2 --> W3[("DELETE user_action_tokens<br/>WHERE expires_at ≤ now OR consumed_at IS NOT NULL")]
    W3 --> W4[("DELETE mfa_challenges<br/>WHERE expires_at ≤ now OR consumed_at IS NOT NULL")]
    W4 --> W5[("DELETE sessions<br/>WHERE expires_at ≤ now − Retention<br/>OR revoked_at ≤ now − Retention<br/>каскад → refresh_tokens")]
    W5 --> W6[("DELETE refresh_tokens<br/>WHERE expires_at ≤ now − Retention<br/>спожиті токени живих сесій лишаються для виявлення повторів")]
    W6 --> W7{"Виняток?"}
    W7 -->|"так"| W8["MaintenanceFailed у лог<br/>хост не зупиняється"]
    W7 -->|"ні"| W9["MaintenanceCompleted<br/>removed = n"]
    W8 --> W1
    W9 --> W1

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef fail fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px,stroke-dasharray: 5 5;

    class W0 core;
    class W1,W2,W3,W4,W5,W6,W9 support;
    class W7 generic;
    class W8 fail;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 15. Композиція в DI

```mermaid
%%{init: {"theme":"base","themeVariables":{"fontFamily":"Segoe UI, Helvetica, Arial, sans-serif","primaryColor":"#1C2128","primaryTextColor":"#FFFFFF","primaryBorderColor":"#A5A5A5","lineColor":"#B1BAC4","edgeLabelBackground":"#1C2128","clusterBkg":"#0D1117","clusterBorder":"#A5A5A5","titleColor":"#FFFFFF"}}}%%
flowchart LR
    accTitle: Композиція модуля в DI
    accDescr: AddIdentityProvider збирає реєстрації з IdentityProviderBuilder, вимагає налаштованої persistence, застосовує реєстрації білдера першими, а потім додає значення за замовчуванням через TryAdd, тому вибір хоста завжди має пріоритет. Опції валідуються на старті застосунку.

    H0(["services.AddIdentityProvider<br/>configuration?, builder → …"]) --> H1["IdentityProviderBuilder"]
    H1 --> H2["Configure, ConfigureJwt, ConfigureSessions, …<br/>біндинг секції Identity або налаштування в коді"]
    H1 --> H3["UsePostgres або UseDbContext<br/>обов'язково, рівно один раз<br/>AddDbContextPool для IdentityDbContext"]
    H1 --> H4["UsePasswordHasher<br/>власна реалізація IPasswordHasher"]
    H1 --> H5["UseJwtBearer<br/>AddAuthentication + JwtBearerOptionsConfigurator"]
    H1 --> H6["UseBackgroundCleanup<br/>IdentityCleanupWorker"]
    H2 & H3 & H4 & H5 & H6 --> H7{"PersistenceConfigured?"}
    H7 -->|"ні"| HE>"InvalidOperationException<br/>persistence не налаштовано"]
    H7 -->|"так"| H8["builder.Apply<br/>реєстрації хоста першими"]
    H8 --> H9(["AddIdentityDefaults через TryAdd<br/>TimeProvider, 7 класів опцій + ValidateOnStart,<br/>PasswordHasher, DummyPasswordHash, JwtProvider,<br/>UserActionTokenService, MfaChallengeService, IdentityMaintenanceService,<br/>IdentityProvider як фасад і 5 сегрегованих інтерфейсів"])

    classDef core fill:#0D1117,color:#FFFFFF,stroke:#FFFFFF,stroke-width:2px;
    classDef support fill:#1C2128,color:#FFFFFF,stroke:#A5A5A5,stroke-width:2px;
    classDef generic fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px;
    classDef fail fill:#2D333B,color:#FFFFFF,stroke:#6E7681,stroke-width:2px,stroke-dasharray: 5 5;

    class H0,H9 core;
    class H1,H2,H3,H4,H5,H6,H8 support;
    class H7 generic;
    class HE fail;

    linkStyle default stroke:#B1BAC4,stroke-width:2px,color:#FFFFFF
```

### 16. Параметри за замовчуванням

| Секція | Параметр | Значення | Де діє |
|--|--|--|--|
| `Jwt` | `AccessTokenLifetime` | 15 хв | діаграма 7 |
| `Jwt` | `ClockSkew` | 30 с | діаграма 10 |
| `Jwt` | `SigningKey` | обов'язковий, від 32 символів | діаграми 7, 10 |
| `Sessions` | `RefreshTokenLifetime` | 7 днів, ковзний | діаграми 7, 8 |
| `Sessions` | `SessionLifetime` | 30 днів, абсолютний | діаграми 7, 8 |
| `Sessions` | `RevokedSessionRetention` | 30 днів | діаграми 9, 14 |
| `Mfa` | `CodeLength` / `ChallengeLifetime` / `MaxAttempts` | 6 цифр / 5 хв / 5 | діаграма 6 |
| `UserActionTokens` | `EmailConfirmationLifetime` / `PasswordResetLifetime` | 24 год / 1 год | діаграми 3, 4, 12 |
| `Passwords` | `WorkFactor` / `MinimumLength` / `MaximumLength` | 12 / 8 / 128 | діаграми 3, 5, 11, 12 |
| `Account` | `RequireConfirmedEmail` | `true` | діаграми 5, 6 |
| `Account` | `MaxFailedAccessAttempts` / `LockoutDuration` | 5 / 15 хв, 0 вимикає lockout | діаграма 5 |
| `Maintenance` | `Interval` | 1 год | діаграма 14 |

### 17. Винятки

| Виняток | Коли | Діаграми |
|--|--|--|
| `UnauthorizedException` | невірні креденшали, невалідний або спожитий токен чи код | 4, 5, 6, 8, 11, 12, 13 |
| `ForbiddenException` | деактивований акаунт, непідтверджений email за політикою | 5, 6, 13 |
| `TooManyRequestsException` | активний lockout | 5 |
| `ConflictException` | дублікат email або username | 3 |
| `BadRequestException` | порушення політики довжини пароля | 3, 11, 12 |
| `NotFoundException` | адмін-операція над невідомим користувачем, видача тікета невідомому користувачу | 13 |

### 18. Відповідність потоків коду

| Потік | Файл |
|--|--|
| 3, 4 | `Base/IdentityProvider.SignUp.cs`, `Base/Tokens/UserActionTokenService.cs` |
| 5, 7, 8, 9 | `Base/IdentityProvider.SignIn.cs`, `Base/IdentityProvider.cs` |
| 6 | `Base/Tokens/MfaChallengeService.cs`, `Base/IdentityProvider.SignIn.cs` |
| 7 | `Base/Security/JwtProvider.cs`, `Base/Security/ClaimsBuilder.cs`, `Base/Security/TokenSecret.cs` |
| 9 | `Base/IdentityProvider.Sessions.cs` |
| 10 | `Base/Authentication/JwtBearerOptionsConfigurator.cs`, `Base/Authentication/ClaimsPrincipalExtensions.cs` |
| 11, 12, 13 | `Base/IdentityProvider.Account.cs`, `Base/IdentityProvider.Administration.cs` |
| 14 | `Base/Maintenance/IdentityCleanupWorker.cs`, `Base/Maintenance/IdentityMaintenanceService.cs` |
| 15 | `Base/Registration/IdentityRegistrar.cs`, `Base/Registration/IdentityProviderBuilder.cs` |
| 2 | `Base/Persistence/IdentityDbContext.cs`, `Base/Persistence/Configurations/*.cs` |
