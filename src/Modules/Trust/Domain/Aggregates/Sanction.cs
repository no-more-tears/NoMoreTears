using NoMoreTears.Modules.Trust.Domain.Entities;
using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Modules.Trust.Domain.Exceptions;
using NoMoreTears.Modules.Trust.Domain.ValueObjects;

namespace NoMoreTears.Modules.Trust.Domain.Aggregates;

/// <summary>
///     Sanction of the ladder imposed only on the basis of an incident with a confirmed violation
///     and recorded in the immutable journal.
/// </summary>
public sealed class Sanction
{
    /// <summary>
    ///     Unique identifier, also used by Masters, Dispatch and Requests to apply the sanction.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     Incident that is the basis of the sanction.
    /// </summary>
    public Guid IncidentId { get; }

    /// <summary>
    ///     Sanctioned participant.
    /// </summary>
    public Party Subject { get; }

    /// <summary>
    ///     Imposed step.
    /// </summary>
    public SanctionStep Step { get; }

    /// <summary>
    ///     Step the ladder required at the moment of imposition.
    /// </summary>
    public SanctionStep LadderStep { get; }

    /// <summary>
    ///     Kind of sanction derived from the step and the role.
    /// </summary>
    public SanctionKind Kind { get; }

    /// <summary>
    ///     Justification of the arbiter.
    /// </summary>
    public Justification Justification { get; }

    /// <summary>
    ///     Arbiter who imposed the sanction.
    /// </summary>
    public Guid ArbiterId { get; }

    /// <summary>
    ///     Moment of imposition.
    /// </summary>
    public DateTimeOffset ImposedAt { get; }

    /// <summary>
    ///     End of the sanction.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; }

    /// <summary>
    ///     Moment the sanction was lifted, for example after an accepted appeal.
    /// </summary>
    public DateTimeOffset? LiftedAt { get; private set; }

    /// <summary>
    ///     Whether the arbiter skipped steps of the ladder because of the severity of the violation.
    /// </summary>
    public bool IsStepSkipped => Step.Value > LadderStep.Value;

    private Sanction(
        Guid id,
        Guid incidentId,
        Party subject,
        SanctionStep step,
        SanctionStep ladderStep,
        Justification justification,
        Guid arbiterId,
        DateTimeOffset imposedAt)
    {
        Id = id;
        IncidentId = incidentId;
        Subject = subject;
        Step = step;
        LadderStep = ladderStep;
        Kind = step.KindFor(subject.Role);
        Justification = justification;
        ArbiterId = arbiterId;
        ImposedAt = imposedAt;
        ExpiresAt = imposedAt + step.Duration;
    }

    private readonly List<AuditEntry> _auditTrail = [];

    /// <summary>
    ///     Immutable audit trail of the sanction.
    /// </summary>
    public IReadOnlyCollection<AuditEntry> AuditTrail => _auditTrail.AsReadOnly();

    /// <summary>
    ///     Imposes a sanction. The step follows the latest step of the subject; a higher step is allowed
    ///     for a severe violation, a lower one is not.
    /// </summary>
    public static Sanction Create(
        Incident incident,
        Party subject,
        SanctionStep step,
        SanctionStep? latestStep,
        Guid arbiterId,
        Justification justification,
        DateTimeOffset imposedAt)
    {
        ArgumentNullException.ThrowIfNull(incident);
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(step);
        ArgumentNullException.ThrowIfNull(justification);

        if (arbiterId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(arbiterId));
        }

        if (!incident.IsViolationConfirmed)
        {
            throw new SanctionNotAllowedException("A sanction requires an incident with a confirmed violation.");
        }

        if (!incident.Involves(subject))
        {
            throw new SanctionNotAllowedException("Only a party of the incident can be sanctioned.");
        }

        var ladderStep = SanctionStep.After(latestStep);

        if (step.Value < ladderStep.Value)
        {
            throw new SanctionNotAllowedException($"The ladder requires at least step {ladderStep.Value}.");
        }

        var sanction = new Sanction(
            Guid.CreateVersion7(imposedAt),
            incident.Id,
            subject,
            step,
            ladderStep,
            justification,
            arbiterId,
            imposedAt);
        sanction.Record(
            AuditAction.SanctionImposed,
            Actor.Of(ActorRole.PlatformSupport, arbiterId),
            justification.Value,
            imposedAt);

        return sanction;
    }

    /// <summary>
    ///     Returns whether the sanction is in effect at the given moment.
    /// </summary>
    public bool IsActiveAt(DateTimeOffset moment)
    {
        return LiftedAt is null && moment >= ImposedAt && (ExpiresAt is null || moment < ExpiresAt);
    }

    /// <summary>
    ///     Lifts the sanction by a decision of a human, for example after an accepted appeal.
    /// </summary>
    public void Lift(
        Actor actor,
        Justification justification,
        DateTimeOffset liftedAt)
    {
        ArgumentNullException.ThrowIfNull(actor);
        ArgumentNullException.ThrowIfNull(justification);

        if (actor.Role is not (ActorRole.PlatformSupport or ActorRole.PlatformAdministrator))
        {
            throw new ActorRoleMismatchException("Only PlatformSupport or PlatformAdministrator can lift a sanction.");
        }

        if (!IsActiveAt(liftedAt))
        {
            throw new SanctionNotActiveException(Id);
        }

        LiftedAt = liftedAt;
        Record(AuditAction.SanctionLifted, actor, justification.Value, liftedAt);
    }

    private void Record(
        AuditAction action,
        Actor actor,
        string? details,
        DateTimeOffset occurredAt)
    {
        _auditTrail.Add(AuditEntry.Create(action, actor, details, occurredAt));
    }
}