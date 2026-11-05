using NoMoreTears.Modules.Trust.Domain.Entities;
using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Modules.Trust.Domain.Exceptions;
using NoMoreTears.Modules.Trust.Domain.ValueObjects;

namespace NoMoreTears.Modules.Trust.Domain.Aggregates;

/// <summary>
///     Temporary measure created by an automatic rule.
/// </summary>
public sealed class TemporaryMeasure
{
    /// <summary>
    ///     Unique identifier, also used by other contexts to apply the effects.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     Rule that created the measure.
    /// </summary>
    public AutomaticRule Rule { get; }

    /// <summary>
    ///     Participant the measure applies to.
    /// </summary>
    public Party Subject { get; }

    /// <summary>
    ///     Related request, if any.
    /// </summary>
    public Guid? RequestId { get; }

    /// <summary>
    ///     Related incident, if any.
    /// </summary>
    public Guid? IncidentId { get; }

    /// <summary>
    ///     Explanation shown to the user.
    /// </summary>
    public Explanation Explanation { get; }

    /// <summary>
    ///     Moment the measure takes effect.
    /// </summary>
    public DateTimeOffset StartsAt { get; }

    /// <summary>
    ///     End of the measure.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; }

    /// <summary>
    ///     Moment by which a human must review the measure.
    /// </summary>
    public DateTimeOffset ReviewDue { get; private set; }

    /// <summary>
    ///     Status of the measure.
    /// </summary>
    public MeasureStatus Status { get; private set; }

    /// <summary>
    ///     Objection of the subject.
    /// </summary>
    public Contest? Contest { get; private set; }

    /// <summary>
    ///     Human who reviewed the measure.
    /// </summary>
    public Actor? Reviewer { get; private set; }

    /// <summary>
    ///     Justification of the human review.
    /// </summary>
    public Justification? ReviewJustification { get; private set; }

    /// <summary>
    ///     Moment the measure was lifted or expired.
    /// </summary>
    public DateTimeOffset? EndedAt { get; private set; }

    /// <summary>
    ///     Reason code shown to the user.
    /// </summary>
    public string ReasonCode => Rule.ToString();

    /// <summary>
    ///     Effects applied by other contexts.
    /// </summary>
    public IReadOnlyList<MeasureEffect> Effects => RuleDefinition.Of(Rule).Effects;

    private readonly List<AuditEntry> _auditTrail = [];

    /// <summary>
    ///     Immutable audit trail of the measure.
    /// </summary>
    public IReadOnlyCollection<AuditEntry> AuditTrail => _auditTrail.AsReadOnly();

    private TemporaryMeasure(
        Guid id,
        AutomaticRule rule,
        Party subject,
        Guid? requestId,
        Guid? incidentId,
        Explanation explanation,
        DateTimeOffset startsAt,
        DateTimeOffset? expiresAt)
    {
        Id = id;
        Rule = rule;
        Subject = subject;
        RequestId = requestId;
        IncidentId = incidentId;
        Explanation = explanation;
        StartsAt = startsAt;
        ExpiresAt = expiresAt;
        ReviewDue = startsAt + TrustRules.HumanReviewPeriod;
        Status = MeasureStatus.PendingReview;
    }

    /// <summary>
    ///     Applies a temporary measure of an automatic rule.
    /// </summary>
    public static TemporaryMeasure Create(
        AutomaticRule rule,
        Party subject,
        Guid? requestId,
        Guid? incidentId,
        Explanation explanation,
        DateTimeOffset startsAt)
    {
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(explanation);

        var definition = RuleDefinition.Of(rule);

        if (subject.Role != definition.SubjectRole)
        {
            throw new PartyMismatchException($"Rule '{rule}' applies only to a participant with role '{definition.SubjectRole}'.");
        }

        if (requestId == Guid.Empty || incidentId == Guid.Empty)
        {
            throw new InvalidIdentifierException(requestId == Guid.Empty ? nameof(requestId) : nameof(incidentId));
        }

        var measure = new TemporaryMeasure(
            Guid.CreateVersion7(startsAt),
            rule,
            subject,
            requestId,
            incidentId,
            explanation,
            startsAt,
            startsAt + definition.Duration);
        measure.Record(AuditAction.MeasureApplied, Actor.System, explanation.Value, startsAt);

        return measure;
    }

    /// <summary>
    ///     Returns whether the measure is in effect at the given moment.
    /// </summary>
    public bool IsActiveAt(DateTimeOffset moment)
    {
        return Status is MeasureStatus.PendingReview or MeasureStatus.Escalated or MeasureStatus.Confirmed
            && moment >= StartsAt
            && (ExpiresAt is null || moment < ExpiresAt);
    }

    /// <summary>
    ///     Returns whether nobody reviewed the measure in time.
    /// </summary>
    public bool IsReviewOverdueAt(DateTimeOffset moment) => Status is MeasureStatus.PendingReview && moment >= ReviewDue;

    /// <summary>
    ///     Contests the measure, once; a confirmed measure goes back to human review.
    /// </summary>
    public void SubmitContest(
        Party subject,
        Statement statement,
        DateTimeOffset contestedAt)
    {
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(statement);
        EnsureStatusIn("contest", MeasureStatus.PendingReview, MeasureStatus.Escalated, MeasureStatus.Confirmed);

        if (subject != Subject)
        {
            throw new PartyMismatchException("Only the subject of the measure can contest it.");
        }

        if (Contest is not null)
        {
            throw new InvalidMeasureStatusException(Status, "contest again");
        }

        Contest = Contest.Of(statement, contestedAt);

        if (Status is MeasureStatus.Confirmed)
        {
            Status = MeasureStatus.PendingReview;
            ReviewDue = contestedAt + TrustRules.HumanReviewPeriod;
        }

        Record(AuditAction.MeasureContested, Actor.Of(subject), statement.Value, contestedAt);
    }

    /// <summary>
    ///     Escalates the measure to PlatformAdministrator when nobody reviewed it in time.
    /// </summary>
    public void Escalate(DateTimeOffset escalatedAt)
    {
        EnsureStatusIn("escalate", MeasureStatus.PendingReview);

        if (escalatedAt < ReviewDue)
        {
            throw new MeasureNotDueException($"The measure can be escalated only after {ReviewDue:O}.");
        }

        Status = MeasureStatus.Escalated;
        Record(AuditAction.MeasureEscalated, Actor.System, null, escalatedAt);
    }

    /// <summary>
    ///     Keeps the measure in effect after human review.
    /// </summary>
    public void Confirm(
        Actor reviewer,
        Justification justification,
        DateTimeOffset confirmedAt)
    {
        EnsureStatusIn("confirm", MeasureStatus.PendingReview, MeasureStatus.Escalated);
        EnsureReviewer(reviewer);
        ArgumentNullException.ThrowIfNull(justification);

        Status = MeasureStatus.Confirmed;
        Reviewer = reviewer;
        ReviewJustification = justification;
        Record(AuditAction.MeasureConfirmed, reviewer, justification.Value, confirmedAt);
    }

    /// <summary>
    ///     Lifts the measure after human review.
    /// </summary>
    public void Lift(
        Actor reviewer,
        Justification justification,
        DateTimeOffset liftedAt)
    {
        EnsureStatusIn("lift", MeasureStatus.PendingReview, MeasureStatus.Escalated, MeasureStatus.Confirmed);
        EnsureReviewer(reviewer);
        ArgumentNullException.ThrowIfNull(justification);

        Status = MeasureStatus.Lifted;
        Reviewer = reviewer;
        ReviewJustification = justification;
        EndedAt = liftedAt;
        Record(AuditAction.MeasureLifted, reviewer, justification.Value, liftedAt);
    }

    /// <summary>
    ///     Ends the measure when its validity has passed.
    /// </summary>
    public void Expire(DateTimeOffset expiredAt)
    {
        EnsureStatusIn("expire", MeasureStatus.PendingReview, MeasureStatus.Escalated, MeasureStatus.Confirmed);

        if (ExpiresAt is null || expiredAt < ExpiresAt)
        {
            throw new MeasureNotDueException("The measure has not reached its end.");
        }

        Status = MeasureStatus.Expired;
        EndedAt = expiredAt;
        Record(AuditAction.MeasureExpired, Actor.System, null, expiredAt);
    }

    private void EnsureReviewer(Actor reviewer)
    {
        ArgumentNullException.ThrowIfNull(reviewer);

        var isAllowed = Status is MeasureStatus.Escalated
            ? reviewer.Role is ActorRole.PlatformAdministrator
            : reviewer.Role is ActorRole.PlatformSupport or ActorRole.PlatformAdministrator;

        if (!isAllowed)
        {
            throw new ActorRoleMismatchException(Status is MeasureStatus.Escalated
                ? "An escalated measure is reviewed by PlatformAdministrator."
                : "A measure is reviewed by PlatformSupport or PlatformAdministrator.");
        }
    }

    private void Record(
        AuditAction action,
        Actor actor,
        string? details,
        DateTimeOffset occurredAt)
    {
        _auditTrail.Add(AuditEntry.Create(action, actor, details, occurredAt));
    }

    private void EnsureStatusIn(
        string action,
        params ReadOnlySpan<MeasureStatus> allowedStatuses)
    {
        foreach (var allowedStatus in allowedStatuses)
        {
            if (Status == allowedStatus)
            {
                return;
            }
        }

        throw new InvalidMeasureStatusException(Status, action);
    }
}