## Identity provider

Shared-модуль автентифікації, авторизації (ролі `UserRole`), MFA та керування сесіями з persistence у PostgreSQL через EF Core.
Модуль **не надсилає листи**: для email-підтвердження, скидання пароля та MFA-коду він повертає тікет/код, який доставляє зовнішній модуль.

| Проєкт | Призначення |
|--|--|
| `Contracts` | DTO, enum-и, назви claim-ів (`IdentityClaimTypes`). Залежить лише від `Shared.Common.Authorization` (через `UserRole`). |
| `Abstractions` | Фасад `IIdentityProvider` та сегреговані інтерфейси: `ISignUpService`, `ISignInService`, `IAccountService`, `ISessionService`, `IUserAdministrationService`, а також `IPasswordHasher`, `IJwtProvider`, `IUserActionTokenService`, `IMfaChallengeService`, `IIdentityMaintenanceService`. |
| `Base` | Реалізація: `IdentityProvider` (partial по capability), `IdentityDbContext` (схема `identity`, PascalCase-назви за замовчуванням EF), опції з валідацією на старті, `IdentityRegistrar` + `IdentityProviderBuilder`, інтеграція з JWT bearer, фонове прибирання. |
| `Migrations` | Design-time factory, EF-міграції та `AddIdentityMigrationManager` для migrations host (у стилі `Shared.Postgres.Migrations`). |

### Реєстрація

```csharp
// 1. Опції з appsettings ("Identity"), Postgres через Shared.Postgres blueprint, bearer-автентифікація, фонове прибирання.
services.AddIdentityProvider(configuration, identity => identity
    .UsePostgres(blueprint => blueprint
        .UseConnectionString("IdentityDb")
        .WithSlowQueryThreshold(TimeSpan.FromMilliseconds(500)))
    .UseJwtBearer()
    .UseBackgroundCleanup(TimeSpan.FromHours(6)));

// 2. Власний DbContextForge<IdentityDbContext> хоста + налаштування опцій у коді.
services.AddIdentityProvider(identity => identity
    .Configure(configuration.GetSection(IdentityRegistrar.SectionName))
    .ConfigureAccount(account => account.RequireConfirmedEmail = false)
    .UsePostgres<IdentityDbForge>()
    .UsePasswordHasher<Argon2PasswordHasher>());

// 3. Пряма конфігурація DbContext (тести, Testcontainers) без bearer-інтеграції.
services.AddIdentityProvider(identity => identity
    .ConfigureJwt(jwt => { jwt.Issuer = "..."; jwt.Audience = "..."; jwt.SigningKey = "..."; })
    .UseDbContext(options => options.UseNpgsql(connectionString)));
```

`UseJwtBearer()` налаштовує `JwtBearerOptions` тими ж `JwtOptions`, якими видаються токени, вимикає inbound claim mapping
і (за замовчуванням) відхиляє access-токени відкликаних сесій через `ISessionService.IsSessionActiveAsync` (claim `sid`).
Хост, який конфігурує bearer сам, бере `IdentityRegistrar.CreateTokenValidationParameters(jwtOptions)`.

### Конфігурація

```json
"Identity": {
  "Jwt": { "Issuer": "nomoretears", "Audience": "nomoretears-api", "SigningKey": "<32+ випадкових символів>", "AccessTokenLifetime": "00:15:00", "ClockSkew": "00:00:30" },
  "Sessions": { "RefreshTokenLifetime": "7.00:00:00", "SessionLifetime": "30.00:00:00", "RevokedSessionRetention": "30.00:00:00" },
  "Mfa": { "CodeLength": 6, "ChallengeLifetime": "00:05:00", "MaxAttempts": 5 },
  "UserActionTokens": { "EmailConfirmationLifetime": "1.00:00:00", "PasswordResetLifetime": "01:00:00" },
  "Passwords": { "WorkFactor": 12, "MinimumLength": 8, "MaximumLength": 128 },
  "Account": { "RequireConfirmedEmail": true, "MaxFailedAccessAttempts": 5, "LockoutDuration": "00:15:00" }
}
```

Усі секції опціональні, окрім `Jwt`. Значення валідуються на старті застосунку (`ValidateOnStart`).

### Потоки

| Потік | Що робить модуль | Що робить хост |
|--|--|--|
| Реєстрація | `RegisterAsync` створює користувача з роллю, повертає `RegistrationResult` з тікетом підтвердження. | Надсилає лист з `EmailConfirmation.Token`, викликає `ConfirmEmailAsync`. |
| Вхід | `LoginAsync` перевіряє пароль з постійним часом, lockout, статус акаунта; повертає токени або `MfaChallenge`. | При MFA надсилає `MfaChallenge.Code` користувачу, повертає клієнту лише `ChallengeToken`, далі `VerifyMfaAsync`. |
| Refresh | `RefreshAsync` ротує refresh-токен умовним UPDATE; повторне використання відкликає сесію. | Зберігає `TokenPair.RefreshToken` у HttpOnly cookie або secure storage. |
| Пароль | `ForgotPasswordAsync` повертає тікет або `null` (без витоку існування акаунта); `ResetPasswordAsync` відкликає сесії. | Надсилає лист, завжди відповідає 200. |
| Сесії | `GetSessionsAsync`, `RevokeSessionAsync`, `LogoutAllAsync`; access-токени відкликаних сесій відхиляються bearer-хуком. | Передає `sid` з `ClaimsPrincipal.GetSessionId()`. |
| Адміністрування | `UpdateRolesAsync`, `DeactivateAsync`, `ReactivateAsync`. | Авторизує виклик (роль `PlatformAdministrator`). |

Винятки: `UnauthorizedException` (невірні креденшали/токени), `ForbiddenException` (неактивний акаунт, непідтверджений email),
`TooManyRequestsException` (lockout), `ConflictException` (дублікат), `BadRequestException` (політика паролів), `NotFoundException` (адмін-операції).

### Міграції

```powershell
# Згенерувати міграцію після зміни моделі (підключення не потрібне).
dotnet ef migrations add <Name> --project Shared/Providers/Identity/Migrations --startup-project Shared/Providers/Identity/Migrations --output-dir Migrations --namespace NoMoreTears.Shared.Providers.Identity.Migrations

# Застосувати з інструментарію (потрібна змінна середовища).
$env:IDENTITY_MIGRATIONS_CONNECTION_STRING = "Host=...;Database=...;Username=...;Password=..."
dotnet ef database update --project Shared/Providers/Identity/Migrations --startup-project Shared/Providers/Identity/Migrations
```

У migrations host: `services.AddIdentityMigrationManager(configuration, "IdentityDb")` та `IDbMigrationManager.ApplyMigrationsAsync`.
Схема `identity` задається в `OnModelCreating`; таблиці, колонки, ключі та індекси мають PascalCase-назви (конвенція EF за замовчуванням). Міграції згенеровано без naming convention, тому в blueprint для Identity не можна вмикати `WithNamingStyle` з іншим значенням, ніж `NamingStyle.Default`.
