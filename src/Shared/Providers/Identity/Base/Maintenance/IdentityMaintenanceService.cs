using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Maintenance;
using NoMoreTears.Shared.Providers.Identity.Diagnostics;
using NoMoreTears.Shared.Providers.Identity.Persistence;
using NoMoreTears.Shared.Providers.Identity.Registration.Configuration;

namespace NoMoreTears.Shared.Providers.Identity.Maintenance;

/// <summary>
///     EF Core backed implementation of <see cref="IIdentityMaintenanceService"/> using bulk deletes.
/// </summary>
internal sealed class IdentityMaintenanceService(
    IdentityDbContext db,
    IOptions<SessionOptions> sessionOptions,
    TimeProvider timeProvider,
    ILogger<IdentityMaintenanceService> logger) : IIdentityMaintenanceService
{
    private readonly SessionOptions _sessions = sessionOptions.Value;

    /// <inheritdoc />
    public async Task<int> PurgeExpiredAsync(CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var retentionBoundary = now - _sessions.RevokedSessionRetention;

        var removed = await db.UserActionTokens
            .Where(x => x.ExpiresAt <= now || x.ConsumedAt != null)
            .ExecuteDeleteAsync(cancellationToken);

        removed += await db.MfaChallenges
            .Where(x => x.ExpiresAt <= now || x.ConsumedAt != null)
            .ExecuteDeleteAsync(cancellationToken);

        // Sessions cascade to their refresh tokens.
        removed += await db.Sessions
            .Where(x => x.ExpiresAt <= retentionBoundary
                || (x.RevokedAt != null && x.RevokedAt <= retentionBoundary))
            .ExecuteDeleteAsync(cancellationToken);

        // Consumed tokens of live sessions are kept until the retention period, so replays stay detectable.
        removed += await db.RefreshTokens
            .Where(x => x.ExpiresAt <= retentionBoundary)
            .ExecuteDeleteAsync(cancellationToken);

        logger.MaintenanceCompleted(removed);

        return removed;
    }
}