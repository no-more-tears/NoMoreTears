using Microsoft.EntityFrameworkCore;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Providers.Identity.Contracts.Passwords;
using NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;
using NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;
using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;
using NoMoreTears.Shared.Providers.Identity.Diagnostics;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;
using NoMoreTears.Shared.Providers.Identity.Security;

namespace NoMoreTears.Shared.Providers.Identity;

/// <summary>
///     Sign-in capability of <see cref="IdentityProvider"/>.
/// </summary>
internal sealed partial class IdentityProvider
{
    /// <inheritdoc />
    public async Task<LoginResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Identifier);
        ArgumentException.ThrowIfNullOrEmpty(request.Password);
        ArgumentNullException.ThrowIfNull(request.Client);

        var now = timeProvider.GetUtcNow();
        var user = await FindUserAsync(request.Identifier, cancellationToken);

        // Always verify, even for unknown users, to keep response timing uniform.
        var verification = VerifyPassword(request.Password, user?.PasswordHash);

        if (user is null)
        {
            throw new UnauthorizedException(IdentityMessages.InvalidCredentials);
        }
        if (user.LockoutEndAt > now)
        {
            throw new TooManyRequestsException(IdentityMessages.AccountLocked);
        }
        if (verification == PasswordVerificationStatus.Failed)
        {
            await RegisterFailedAccessAsync(user.Id, now, cancellationToken);
            throw new UnauthorizedException(IdentityMessages.InvalidCredentials);
        }

        EnsureCanSignIn(user);

        if (verification == PasswordVerificationStatus.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(request.Password);
        }

        user.FailedAccessAttempts = 0;
        user.LockoutEndAt = null;

        if (user.MfaEnabled)
        {
            await db.SaveChangesAsync(cancellationToken);
            var challenge = await mfaChallenges.CreateAsync(user.Id, request.Client, cancellationToken);

            return LoginResult.MfaRequired(challenge);
        }

        user.LastLoginAt = now;
        var tokens = await StartSessionAsync(user, request.Client, mfaVerified: false, now, cancellationToken);

        return LoginResult.Authenticated(tokens);
    }

    /// <inheritdoc />
    public async Task<TokenPair> VerifyMfaAsync(
        VerifyMfaRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ChallengeToken);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Code);
        ArgumentNullException.ThrowIfNull(request.Client);

        var userId = await mfaChallenges.VerifyAsync(request.ChallengeToken, request.Code, request.Client, cancellationToken)
            ?? throw new UnauthorizedException(IdentityMessages.InvalidMfaCode);

        var user = await FindUserAsync(userId, cancellationToken)
            ?? throw new UnauthorizedException(IdentityMessages.InvalidCredentials);

        EnsureCanSignIn(user);

        var now = timeProvider.GetUtcNow();
        user.LastLoginAt = now;

        return await StartSessionAsync(user, request.Client, mfaVerified: true, now, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TokenPair> RefreshAsync(
        RefreshRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RefreshToken);
        ArgumentNullException.ThrowIfNull(request.Client);

        var now = timeProvider.GetUtcNow();
        var tokenHash = TokenSecret.Hash(request.RefreshToken);

        var current = await db.RefreshTokens
            .Include(x => x.Session)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken)
            ?? throw new UnauthorizedException(IdentityMessages.InvalidRefreshToken);

        var session = current.Session;

        if (current.ConsumedAt is not null)
        {
            await HandleReuseAsync(session, now, cancellationToken);
            throw new UnauthorizedException(IdentityMessages.InvalidRefreshToken);
        }
        if (session.RevokedAt is not null || session.ExpiresAt <= now || current.ExpiresAt <= now)
        {
            throw new UnauthorizedException(IdentityMessages.InvalidRefreshToken);
        }

        var user = await FindUserAsync(session.UserId, cancellationToken);

        if (user is not { IsActive: true })
        {
            throw new UnauthorizedException(IdentityMessages.InvalidRefreshToken);
        }

        var (secret, next) = CreateRefreshToken(session, now);

        // Exactly one concurrent refresh wins; the others are treated as replays.
        var rotated = await db.RefreshTokens
            .Where(x => x.Id == current.Id && x.ConsumedAt == null)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(x => x.ConsumedAt, now)
                    .SetProperty(x => x.ReplacedByTokenId, next.Id),
                cancellationToken);

        if (rotated != 1)
        {
            await HandleReuseAsync(session, now, cancellationToken);
            throw new UnauthorizedException(IdentityMessages.InvalidRefreshToken);
        }

        session.LastActivityAt = now;
        session.IpAddress = request.Client.IpAddress ?? session.IpAddress;

        db.RefreshTokens.Add(next);
        await db.SaveChangesAsync(cancellationToken);

        return IssueTokens(user, session, secret, next);
    }

    /// <inheritdoc />
    public async Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        var tokenHash = TokenSecret.Hash(refreshToken);

        var sessionId = await db.RefreshTokens
            .Where(x => x.TokenHash == tokenHash)
            .Select(x => (Guid?)x.SessionId)
            .SingleOrDefaultAsync(cancellationToken);

        if (sessionId is { } id)
        {
            await MarkSessionRevokedAsync(id, SessionRevocationReason.Logout, timeProvider.GetUtcNow(), cancellationToken);
        }
    }

    /// <inheritdoc />
    public Task LogoutAllAsync(
        Guid userId,
        CancellationToken cancellationToken) =>
        RevokeSessionsAsync(
            userId,
            SessionRevocationReason.LogoutAll,
            timeProvider.GetUtcNow(),
            exceptSessionId: null,
            cancellationToken);

    /// <summary>
    ///     Counts a failed attempt and locks the account once the configured threshold is reached.
    /// </summary>
    private async Task RegisterFailedAccessAsync(
        Guid userId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (_account.MaxFailedAccessAttempts <= 0)
        {
            return;
        }

        var maxAttempts = _account.MaxFailedAccessAttempts;
        var lockoutEndAt = now + _account.LockoutDuration;

        await db.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(x => x.FailedAccessAttempts, x => x.FailedAccessAttempts + 1),
                cancellationToken);

        var locked = await db.Users
            .Where(x => x.Id == userId && x.FailedAccessAttempts >= maxAttempts)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(x => x.FailedAccessAttempts, 0)
                    .SetProperty(x => x.LockoutEndAt, lockoutEndAt),
                cancellationToken);

        if (locked > 0)
        {
            logger.AccountLocked(userId, lockoutEndAt);
        }
    }

    /// <summary>
    ///     A rotated token was presented again: the session is considered compromised and revoked.
    /// </summary>
    private async Task HandleReuseAsync(
        SessionEntity session,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var revoked = await MarkSessionRevokedAsync(session.Id, SessionRevocationReason.ReuseDetected, now, cancellationToken);

        if (revoked > 0)
        {
            logger.RefreshTokenReuseDetected(session.UserId, session.Id);
        }
    }
}