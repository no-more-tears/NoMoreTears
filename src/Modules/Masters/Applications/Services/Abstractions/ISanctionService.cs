using NoMoreTears.Modules.Masters.Applications.Services.Results;

namespace NoMoreTears.Modules.Masters.Applications.Services.Abstractions;

/// <summary>
///     Applies sanctions and temporary measures of Trust to master profiles.
/// </summary>
public interface ISanctionService
{
    Task<SanctionOutcome?> ApplySanctionAsync(
        Guid masterId,
        Guid sanctionId,
        string kind,
        DateTimeOffset imposedAt,
        DateTimeOffset? expiresAt,
        CancellationToken cancellationToken);

    Task<SanctionOutcome?> LiftSanctionAsync(
        Guid masterId,
        Guid sanctionId,
        DateTimeOffset liftedAt,
        CancellationToken cancellationToken);

    Task<MeasureOutcome?> ApplyMeasureAsync(
        Guid masterId,
        Guid measureId,
        string rule,
        bool suspendsFeed,
        bool holdsVerification,
        DateTimeOffset appliedAt,
        DateTimeOffset? expiresAt,
        CancellationToken cancellationToken);

    Task<MeasureOutcome?> LiftMeasureAsync(
        Guid masterId,
        Guid measureId,
        bool suspendsFeed,
        bool holdsVerification,
        DateTimeOffset liftedAt,
        CancellationToken cancellationToken);
}