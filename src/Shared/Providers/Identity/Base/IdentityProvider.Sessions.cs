using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;

namespace NoMoreTears.Shared.Providers.Identity;

/// <summary>
///     Session management capability of <see cref="IdentityProvider"/>.
/// </summary>
internal sealed partial class IdentityProvider
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<SessionInfo>> GetSessionsAsync(
        Guid userId,
        Guid? currentSessionId,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();

        return await db.Sessions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > now)
            .OrderByDescending(x => x.LastActivityAt)
            .Select(x => new SessionInfo(
                x.Id,
                x.CreatedAt,
                x.LastActivityAt,
                x.ExpiresAt,
                x.MfaVerified,
                x.IpAddress,
                x.UserAgent,
                x.DeviceId,
                x.Id == currentSessionId))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RevokeSessionAsync(
        Guid userId,
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        const SessionRevocationReason reason = SessionRevocationReason.RevokedByUser;

        var count = await db.Sessions
            .Where(x => x.Id == sessionId && x.UserId == userId && x.RevokedAt == null)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(x => x.RevokedAt, now)
                    .SetProperty(x => x.RevocationReason, reason),
                cancellationToken);

        if (count > 0)
        {
            logger.LogInformation("Revoked {Count} session(s) of user {UserId}. Reason={Reason}", count, userId, reason);
        }
    }

    /// <inheritdoc />
    public Task<bool> IsSessionActiveAsync(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();

        return db.Sessions.AnyAsync(
            x => x.Id == sessionId && x.RevokedAt == null && x.ExpiresAt > now,
            cancellationToken);
    }
}