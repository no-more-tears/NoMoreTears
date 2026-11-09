using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Masters.Applications.Services.Abstractions;
using NoMoreTears.Modules.Masters.Applications.Services.Extensions;
using NoMoreTears.Modules.Masters.Applications.Services.Mappings;
using NoMoreTears.Modules.Masters.Applications.Services.Results;
using NoMoreTears.Modules.Masters.Domain;
using NoMoreTears.Modules.Masters.Domain.Aggregates;
using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Masters.Applications.Services;

/// <summary>
///     Applies sanctions and temporary measures of Trust to master profiles.
/// </summary>
public sealed class SanctionService(
    IRepository<Master, Guid> masters,
    TimeProvider timeProvider,
    ILogger<SanctionService> logger) : ISanctionService
{
    /// <inheritdoc />
    public async Task<SanctionOutcome?> ApplySanctionAsync(
        Guid masterId,
        Guid sanctionId,
        string kind,
        DateTimeOffset imposedAt,
        DateTimeOffset? expiresAt,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<SanctionKind>(kind, ignoreCase: true, out var sanctionKind))
        {
            logger.LogWarning("Sanction {SanctionId} of kind {Kind} does not apply to masters and is skipped", sanctionId, kind);
            return null;
        }

        var master = await masters.FindTrackedAsync(masterId, cancellationToken);

        if (master is null || master.HasSanction(sanctionId))
        {
            return null;
        }

        var verificationChanged = false;

        switch (sanctionKind)
        {
            case SanctionKind.Warning:
                master.Warn(sanctionId, imposedAt);
                break;
            case SanctionKind.Restriction:
                master.Restrict(sanctionId, imposedAt, expiresAt ?? imposedAt.AddDays(MasterRules.DefaultRestrictionDurationInDays));
                break;
            case SanctionKind.Suspension:
                master.Suspend(sanctionId, imposedAt, expiresAt);
                break;
            case SanctionKind.Deverification when master.Verification is VerificationStatus.Verified:
                master.RevokeVerification(sanctionId, imposedAt);
                verificationChanged = true;
                break;
            default:
                logger.LogWarning(
                    "Sanction {SanctionId} skipped: master {MasterId} with status {Verification} cannot be deverified",
                    sanctionId, masterId, master.Verification);
                return null;
        }

        await masters.UpdateAsync(master, cancellationToken);

        logger.LogInformation("Sanction {SanctionId} ({Kind}) applied to master {MasterId}", sanctionId, sanctionKind, masterId);
        return ToOutcome(master, sanctionId, verificationChanged);
    }

    /// <inheritdoc />
    public async Task<SanctionOutcome?> LiftSanctionAsync(
        Guid masterId,
        Guid sanctionId,
        DateTimeOffset liftedAt,
        CancellationToken cancellationToken)
    {
        var master = await masters.FindTrackedAsync(masterId, cancellationToken);
        var sanction = master?.Sanctions.FirstOrDefault(applied => applied.Id == sanctionId);

        if (master is null || sanction is null || sanction.LiftedAt is not null)
        {
            return null;
        }

        var wasRevoked = master.Verification is VerificationStatus.Revoked;

        master.LiftSanction(sanctionId, liftedAt);
        await masters.UpdateAsync(master, cancellationToken);

        logger.LogInformation("Sanction {SanctionId} of master {MasterId} lifted", sanctionId, masterId);
        return ToOutcome(master, sanctionId, wasRevoked && master.Verification is VerificationStatus.Verified);
    }

    /// <inheritdoc />
    public async Task<MeasureOutcome?> ApplyMeasureAsync(
        Guid masterId,
        Guid measureId,
        string rule,
        bool suspendsFeed,
        bool holdsVerification,
        DateTimeOffset appliedAt,
        DateTimeOffset? expiresAt,
        CancellationToken cancellationToken)
    {
        var master = await masters.FindTrackedAsync(masterId, cancellationToken);

        if (master is null)
        {
            return null;
        }

        var suspended = suspendsFeed && !master.HasSanction(measureId);
        var held = holdsVerification && master.VerificationHold is null;

        if (!suspended && !held)
        {
            return null;
        }

        if (suspended)
        {
            master.Suspend(measureId, appliedAt, expiresAt);
        }

        if (held)
        {
            master.PlaceVerificationHold(Reason.Of($"Automatic rule '{rule}' requires a human check."), appliedAt);
        }

        await masters.UpdateAsync(master, cancellationToken);

        logger.LogInformation("Temporary measure {MeasureId} ({Rule}) applied to master {MasterId}", measureId, rule, masterId);
        return ToMeasureOutcome(master, suspended ? measureId : null, held);
    }

    /// <inheritdoc />
    public async Task<MeasureOutcome?> LiftMeasureAsync(
        Guid masterId,
        Guid measureId,
        bool suspendsFeed,
        bool holdsVerification,
        DateTimeOffset liftedAt,
        CancellationToken cancellationToken)
    {
        var master = await masters.FindTrackedAsync(masterId, cancellationToken);

        if (master is null)
        {
            return null;
        }

        var suspension = master.Sanctions.FirstOrDefault(applied => applied.Id == measureId);
        var liftSuspension = suspendsFeed && suspension is { LiftedAt: null };
        var releaseHold = holdsVerification && master.VerificationHold is not null;

        if (!liftSuspension && !releaseHold)
        {
            return null;
        }

        if (liftSuspension)
        {
            master.LiftSanction(measureId, liftedAt);
        }

        if (releaseHold)
        {
            master.ReleaseVerificationHold(liftedAt);
        }

        await masters.UpdateAsync(master, cancellationToken);

        logger.LogInformation("Temporary measure {MeasureId} of master {MasterId} lifted", measureId, masterId);
        return ToMeasureOutcome(master, liftSuspension ? measureId : null, releaseHold);
    }

    private SanctionOutcome ToOutcome(
        Master master,
        Guid sanctionId,
        bool verificationChanged)
    {
        var now = timeProvider.GetUtcNow();
        var sanction = master.Sanctions.First(applied => applied.Id == sanctionId);

        return new SanctionOutcome(master.ToResponse(now), sanction.ToResponse(now), verificationChanged);
    }

    private MeasureOutcome ToMeasureOutcome(
        Master master,
        Guid? suspensionId,
        bool holdChanged)
    {
        var now = timeProvider.GetUtcNow();
        var suspension = suspensionId is null
            ? null
            : master.Sanctions.First(applied => applied.Id == suspensionId).ToResponse(now);

        return new MeasureOutcome(master.ToResponse(now), suspension, holdChanged);
    }
}