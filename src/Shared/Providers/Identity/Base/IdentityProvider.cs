using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Providers.Identity.Abstractions;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Security;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Tokens;
using NoMoreTears.Shared.Providers.Identity.Contracts.Common;
using NoMoreTears.Shared.Providers.Identity.Contracts.Passwords;
using NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;
using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;
using NoMoreTears.Shared.Providers.Identity.Diagnostics;
using NoMoreTears.Shared.Providers.Identity.Persistence;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;
using NoMoreTears.Shared.Providers.Identity.Registration.Configuration;
using NoMoreTears.Shared.Providers.Identity.Security;
using Npgsql;

namespace NoMoreTears.Shared.Providers.Identity;

/// <summary>
///     Default implementation of <see cref="IIdentityProvider"/> backed by <see cref="IdentityDbContext"/>.
/// </summary>
/// <remarks>
///     Every mutation that has to be exclusive (token rotation, challenge consumption, revocation)
///     is a conditional update, so the provider never opens user-initiated transactions and stays
///     compatible with the retrying execution strategy of the Postgres module.
///     The implementation is split into partial files, one per capability.
/// </remarks>
internal sealed partial class IdentityProvider(
    IdentityDbContext db,
    IPasswordHasher passwordHasher,
    DummyPasswordHash dummyPasswordHash,
    IJwtProvider jwtProvider,
    IUserActionTokenService actionTokens,
    IMfaChallengeService mfaChallenges,
    IOptions<SessionOptions> sessionOptions,
    IOptions<PasswordOptions> passwordOptions,
    IOptions<AccountOptions> accountOptions,
    TimeProvider timeProvider,
    ILogger<IdentityProvider> logger) : IIdentityProvider
{
    private const int RefreshTokenSizeInBytes = 32;

    private readonly SessionOptions _sessions = sessionOptions.Value;
    private readonly PasswordOptions _passwords = passwordOptions.Value;
    private readonly AccountOptions _account = accountOptions.Value;

    /// <summary>
    ///     Loads a tracked user with roles by email address or username.
    /// </summary>
    private Task<UserEntity?> FindUserAsync(
        string identifier,
        CancellationToken cancellationToken)
    {
        var normalized = Normalize(identifier);

        return db.Users
            .Include(x => x.Roles)
            .SingleOrDefaultAsync(
                x => x.NormalizedEmail == normalized || x.NormalizedUsername == normalized,
                cancellationToken);
    }

    /// <summary>
    ///     Loads a tracked user with roles by identifier.
    /// </summary>
    private Task<UserEntity?> FindUserAsync(
        Guid userId,
        CancellationToken cancellationToken) =>
        db.Users
            .Include(x => x.Roles)
            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);

    /// <summary>
    ///     Loads a tracked user with roles by identifier or throws <see cref="NotFoundException"/>.
    /// </summary>
    private async Task<UserEntity> RequireUserAsync(
        Guid userId,
        CancellationToken cancellationToken) =>
        await FindUserAsync(userId, cancellationToken)
            ?? throw new NotFoundException(IdentityMessages.UserNotFound);

    /// <summary>
    ///     Verifies the password against the stored hash, or against the dummy hash when the
    ///     user is unknown so that the response time does not reveal account existence.
    /// </summary>
    private PasswordVerificationStatus VerifyPassword(
        string password,
        string? passwordHash) =>
        passwordHasher.VerifyHashedPassword(password, passwordHash ?? dummyPasswordHash.Value);

    private void EnsurePasswordPolicy(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        if (password.Length < _passwords.MinimumLength)
        {
            throw new BadRequestException(IdentityMessages.PasswordTooShort(_passwords.MinimumLength));
        }
        if (password.Length > _passwords.MaximumLength)
        {
            throw new BadRequestException(IdentityMessages.PasswordTooLong(_passwords.MaximumLength));
        }
    }

    private void EnsureCanSignIn(UserEntity user)
    {
        if (!user.IsActive)
        {
            throw new ForbiddenException(IdentityMessages.AccountDeactivated);
        }
        if (_account.RequireConfirmedEmail && !user.EmailConfirmed)
        {
            throw new ForbiddenException(IdentityMessages.EmailNotConfirmed);
        }
    }

    /// <summary>
    ///     Starts a session for the user and issues the first token pair.
    /// </summary>
    private async Task<TokenPair> StartSessionAsync(
        UserEntity user,
        ClientContext client,
        bool mfaVerified,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var session = new SessionEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            CreatedAt = now,
            LastActivityAt = now,
            ExpiresAt = now + _sessions.SessionLifetime,
            MfaVerified = mfaVerified,
            IpAddress = client.IpAddress,
            UserAgent = client.UserAgent,
            DeviceId = client.DeviceId
        };

        var (secret, token) = CreateRefreshToken(session, now);

        db.Sessions.Add(session);
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync(cancellationToken);

        return IssueTokens(user, session, secret, token);
    }

    /// <summary>
    ///     Creates a refresh token whose sliding lifetime never exceeds the absolute session lifetime.
    /// </summary>
    private (string Secret, RefreshTokenEntity Token) CreateRefreshToken(
        SessionEntity session,
        DateTimeOffset now)
    {
        var secret = TokenSecret.Generate(RefreshTokenSizeInBytes);
        var slidingExpiresAt = now + _sessions.RefreshTokenLifetime;

        var token = new RefreshTokenEntity
        {
            Id = Guid.CreateVersion7(),
            SessionId = session.Id,
            Session = session,
            TokenHash = TokenSecret.Hash(secret),
            CreatedAt = now,
            ExpiresAt = slidingExpiresAt < session.ExpiresAt ? slidingExpiresAt : session.ExpiresAt
        };

        return (secret, token);
    }

    private TokenPair IssueTokens(
        UserEntity user,
        SessionEntity session,
        string refreshSecret,
        RefreshTokenEntity refreshToken)
    {
        var accessToken = jwtProvider.CreateAccessToken(ClaimsBuilder.Build(user, session));

        return new TokenPair(
            accessToken.Value,
            accessToken.ExpiresAt,
            refreshSecret,
            refreshToken.ExpiresAt,
            session.Id);
    }

    /// <summary>
    ///     Revokes the active sessions of the user, optionally keeping a single one alive.
    /// </summary>
    private async Task RevokeSessionsAsync(
        Guid userId,
        SessionRevocationReason reason,
        DateTimeOffset now,
        Guid? exceptSessionId,
        CancellationToken cancellationToken)
    {
        var count = await db.Sessions
            .Where(x => x.UserId == userId
                && x.RevokedAt == null
                && (exceptSessionId == null || x.Id != exceptSessionId))
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(x => x.RevokedAt, now)
                    .SetProperty(x => x.RevocationReason, reason),
                cancellationToken);

        if (count > 0)
        {
            logger.SessionsRevoked(userId, reason, count);
        }
    }

    /// <summary>
    ///     Revokes a single session regardless of its owner.
    /// </summary>
    /// <returns>Number of affected rows: one when revoked, zero when already revoked or unknown.</returns>
    private Task<int> MarkSessionRevokedAsync(
        Guid sessionId,
        SessionRevocationReason reason,
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        db.Sessions
            .Where(x => x.Id == sessionId && x.RevokedAt == null)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(x => x.RevokedAt, now)
                    .SetProperty(x => x.RevocationReason, reason),
                cancellationToken);

    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
}