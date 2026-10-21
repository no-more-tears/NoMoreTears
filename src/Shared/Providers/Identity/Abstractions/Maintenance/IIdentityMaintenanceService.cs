namespace NoMoreTears.Shared.Providers.Identity.Abstractions.Maintenance;

/// <summary>
///     Housekeeping over identity storage.
/// </summary>
/// <remarks>
///     Expired records are harmless and are never consulted by request processing,
///     so cleanup can run on any schedule chosen by the host.
/// </remarks>
public interface IIdentityMaintenanceService
{
    /// <summary>
    ///     Removes expired and consumed tokens, challenges and sessions past their retention period.
    /// </summary>
    /// <returns>Number of removed rows.</returns>
    Task<int> PurgeExpiredAsync(CancellationToken cancellationToken);
}