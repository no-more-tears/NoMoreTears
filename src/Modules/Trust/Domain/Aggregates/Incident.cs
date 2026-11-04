using NoMoreTears.Modules.Trust.Domain.Entities;
using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Modules.Trust.Domain.Exceptions;
using NoMoreTears.Modules.Trust.Domain.ValueObjects;

namespace NoMoreTears.Modules.Trust.Domain.Aggregates;

/// <summary>
///     Incident opened by an admissible complaint or a fraud signal.
/// </summary>
public sealed class Incident
{
    /// <summary>
    ///     Unique identifier of the incident.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     Origin of the incident.
    /// </summary>
    public IncidentKind Kind { get; }

    /// <summary>
    ///     Request the incident is about, if any.
    /// </summary>
    public Guid? RequestId { get; }

    /// <summary>
    ///     Participant whose behavior is examined.
    /// </summary>
    public Party Subject { get; }

    /// <summary>
    ///     Participant who complained.
    /// </summary>
    public Party? Complainant { get; }

    /// <summary>
    ///     Priority of the review.
    /// </summary>
    public IncidentPriority Priority { get; }

    /// <summary>
    ///     Complaint text or description of the fraud signal.
    /// </summary>
    public Statement Summary { get; }

    /// <summary>
    ///     Status in arbitration.
    /// </summary>
    public IncidentStatus Status { get; private set; }

    /// <summary>
    ///     Moment the incident was opened.
    /// </summary>
    public DateTimeOffset OpenedAt { get; }

    /// <summary>
    ///     End of evidence collection from both parties.
    /// </summary>
    public DateTimeOffset EvidenceDeadline { get; }

    /// <summary>
    ///     SLA of the first reaction of an arbiter.
    /// </summary>
    public DateTimeOffset FirstReactionDue { get; }

    /// <summary>
    ///     SLA of the decision.
    /// </summary>
    public DateTimeOffset DecisionDue { get; }

    /// <summary>
    ///     Arbiter who took the incident into review.
    /// </summary>
    public Guid? ArbiterId { get; private set; }

    /// <summary>
    ///     Moment of the first reaction of an arbiter.
    /// </summary>
    public DateTimeOffset? TakenIntoReviewAt { get; private set; }

    /// <summary>
    ///     Decision of the arbiter.
    /// </summary>
    public Decision? Decision { get; private set; }

    /// <summary>
    ///     End of the appeal window.
    /// </summary>
    public DateTimeOffset? AppealDeadline { get; private set; }

    /// <summary>
    ///     Appeal against the decision.
    /// </summary>
    public Appeal? Appeal { get; private set; }

    /// <summary>
    ///     Decision on the appeal.
    /// </summary>
    public AppealDecision? AppealDecision { get; private set; }

    /// <summary>
    ///     Moment the incident was closed.
    /// </summary>
    public DateTimeOffset? ClosedAt { get; private set; }

    private readonly List<Evidence> _evidence = [];

    /// <summary>
    ///     Evidence submitted by the parties.
    /// </summary>
    public IReadOnlyCollection<Evidence> Evidence => _evidence.AsReadOnly();

    private readonly List<AuditEntry> _auditTrail = [];

    /// <summary>
    ///     Immutable audit trail of the incident.
    /// </summary>
    public IReadOnlyCollection<AuditEntry> AuditTrail => _auditTrail.AsReadOnly();

    /// <summary>
    ///     Outcome after a possible appeal.
    /// </summary>
    public DecisionOutcome? FinalOutcome => Decision is null
        ? null
        : AppealDecision?.Outcome is AppealOutcome.DecisionOverturned
            ? Decision.Outcome is DecisionOutcome.ViolationConfirmed
                ? DecisionOutcome.ViolationNotConfirmed
                : DecisionOutcome.ViolationConfirmed
            : Decision.Outcome;

    /// <summary>
    ///     Whether a violation is confirmed, which is the only basis for a sanction.
    /// </summary>
    public bool IsViolationConfirmed => FinalOutcome is DecisionOutcome.ViolationConfirmed;

    /// <summary>
    ///     Whether this is a complaint rejected by arbitration, counted for FR-TRS-11.
    /// </summary>
    public bool IsComplaintRejected => Kind is IncidentKind.Complaint && FinalOutcome is DecisionOutcome.ViolationNotConfirmed;

    private Incident(
        Guid id,
        IncidentKind kind,
        Guid? requestId,
        Party subject,
        Party? complainant,
        IncidentPriority priority,
        Statement summary,
        DateTimeOffset openedAt)
    {
        Id = id;
        Kind = kind;
        RequestId = requestId;
        Subject = subject;
        Complainant = complainant;
        Priority = priority;
        Summary = summary;
        OpenedAt = openedAt;
        Status = IncidentStatus.Open;
        EvidenceDeadline = openedAt + TrustRules.EvidenceCollectionPeriod;
        FirstReactionDue = openedAt + TrustRules.FirstReactionPeriod;
        DecisionDue = openedAt + TrustRules.DecisionPeriod;
    }

    /// <summary>
    ///     Opens an incident from an admissible complaint of a participant.
    /// </summary>
    public static Incident Create(
        RequestSnapshot request,
        Party complainant,
        Statement statement,
        IReadOnlyCollection<EvidenceFile> files,
        int rejectedComplaintsLast90Days,
        DateTimeOffset submittedAt)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(complainant);
        ArgumentNullException.ThrowIfNull(statement);
        ArgumentNullException.ThrowIfNull(files);
        ArgumentOutOfRangeException.ThrowIfNegative(rejectedComplaintsLast90Days);

        if (request.State is not (RequestState.InProgress or RequestState.Completed or RequestState.Failed))
        {
            throw new ComplaintNotAllowedException("A complaint is possible only for a request in progress, completed or failed.");
        }

        if (submittedAt > request.FinishedAt + TrustRules.ComplaintWindow)
        {
            throw new ComplaintNotAllowedException(
                $"A complaint can be submitted within {TrustRules.ComplaintWindow.TotalDays} days after the request finished.");
        }

        var subject = request.CounterpartyOf(complainant);
        var requiresMedia = SignalRules.RequiresMediaEvidence(rejectedComplaintsLast90Days);

        if (requiresMedia && files.Count == 0)
        {
            throw new MediaEvidenceRequiredException();
        }

        var incident = new Incident(
            Guid.CreateVersion7(submittedAt),
            IncidentKind.Complaint,
            request.RequestId,
            subject,
            complainant,
            requiresMedia ? IncidentPriority.Low : IncidentPriority.Normal,
            statement,
            submittedAt);

        incident._evidence.Add(Entities.Evidence.Create(complainant, statement, files, submittedAt));
        incident.Record(AuditAction.IncidentOpened, Actor.Of(complainant), statement.Value, submittedAt);

        return incident;
    }

    /// <summary>
    ///     Opens an incident from a fraud signal.
    /// </summary>
    public static Incident Create(
        IncidentKind kind,
        Party subject,
        Guid? requestId,
        Statement summary,
        DateTimeOffset openedAt)
    {
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(summary);

        if (kind is IncidentKind.Complaint || !Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind), kind, "A complaint is opened from a request and a complainant.");
        }

        if (requestId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(requestId));
        }

        var incident = new Incident(
            Guid.CreateVersion7(openedAt),
            kind,
            requestId,
            subject,
            null,
            IncidentPriority.Normal,
            summary,
            openedAt);
        incident.Record(AuditAction.IncidentOpened, Actor.System, summary.Value, openedAt);

        return incident;
    }

    /// <summary>
    ///     Adds evidence of a party during the evidence collection.
    /// </summary>
    public void SubmitEvidence(
        Party party,
        Statement statement,
        IReadOnlyCollection<EvidenceFile> files,
        DateTimeOffset submittedAt)
    {
        ArgumentNullException.ThrowIfNull(party);
        EnsureStatusIn("submit evidence to", IncidentStatus.Open, IncidentStatus.InReview);
        EnsureParty(party);

        if (submittedAt > EvidenceDeadline)
        {
            throw new EvidenceCollectionClosedException(EvidenceDeadline);
        }

        _evidence.Add(Entities.Evidence.Create(party, statement, files, submittedAt));
    }

    /// <summary>
    ///     Records the first reaction of an arbiter, who takes the incident into review.
    /// </summary>
    public void TakeIntoReview(
        Guid arbiterId,
        DateTimeOffset takenAt)
    {
        EnsureStatusIn("take into review", IncidentStatus.Open);

        ArbiterId = arbiterId;
        TakenIntoReviewAt = takenAt;
        Status = IncidentStatus.InReview;

        Record(AuditAction.IncidentTakenIntoReview, Actor.Of(ActorRole.PlatformSupport, arbiterId), null, takenAt);
    }

    /// <summary>
    ///     Makes the arbitration decision with the mandatory justification after evidence collection.
    /// </summary>
    public void Decide(
        Guid arbiterId,
        DecisionOutcome outcome,
        Justification justification,
        DateTimeOffset decidedAt)
    {
        EnsureStatusIn("decide", IncidentStatus.InReview);

        if (decidedAt < EvidenceDeadline)
        {
            throw new EvidenceCollectionNotFinishedException(EvidenceDeadline);
        }

        Decision = Decision.Of(arbiterId, outcome, justification, decidedAt);
        AppealDeadline = decidedAt + TrustRules.AppealWindow;
        Status = IncidentStatus.Decided;

        Record(AuditAction.IncidentDecided, Actor.Of(ActorRole.PlatformSupport, arbiterId), justification.Value, decidedAt);
    }

    /// <summary>
    ///     Appeals the decision, once and within 7 days.
    /// </summary>
    public void SubmitAppeal(
        Party appellant,
        Statement statement,
        DateTimeOffset submittedAt)
    {
        ArgumentNullException.ThrowIfNull(appellant);
        EnsureStatusIn("appeal the decision of", IncidentStatus.Decided);
        EnsureParty(appellant);

        if (submittedAt > AppealDeadline)
        {
            throw new AppealNotAllowedException($"An appeal can be submitted until {AppealDeadline:O}.");
        }

        Appeal = Appeal.Of(appellant, statement, submittedAt);
        Status = IncidentStatus.UnderAppeal;

        Record(AuditAction.DecisionAppealed, Actor.Of(appellant), statement.Value, submittedAt);
    }

    /// <summary>
    ///     Decides on the appeal; the incident becomes final.
    /// </summary>
    public void DecideAppeal(
        Guid administratorId,
        AppealOutcome outcome,
        Justification justification,
        DateTimeOffset decidedAt)
    {
        EnsureStatusIn("decide the appeal of", IncidentStatus.UnderAppeal);

        AppealDecision = AppealDecision.Of(administratorId, outcome, justification, decidedAt);
        Status = IncidentStatus.Closed;
        ClosedAt = decidedAt;

        Record(
            AuditAction.AppealDecided,
            Actor.Of(ActorRole.PlatformAdministrator, administratorId),
            justification.Value,
            decidedAt);
    }

    /// <summary>
    ///     Closes the incident when the appeal window passed without an appeal.
    /// </summary>
    public void Close(DateTimeOffset closedAt)
    {
        EnsureStatusIn("close", IncidentStatus.Decided);

        if (closedAt <= AppealDeadline)
        {
            throw new AppealNotAllowedException($"The appeal window is open until {AppealDeadline:O}.");
        }

        Status = IncidentStatus.Closed;
        ClosedAt = closedAt;
        Record(AuditAction.IncidentClosed, Actor.System, null, closedAt);
    }

    /// <summary>
    ///     Returns whether the party is the subject or the complainant.
    /// </summary>
    public bool Involves(Party party) => party == Subject || party == Complainant;

    /// <summary>
    ///     Returns whether the first reaction SLA is breached at the given moment.
    /// </summary>
    public bool IsFirstReactionOverdueAt(DateTimeOffset moment) => TakenIntoReviewAt is null && moment > FirstReactionDue;

    /// <summary>
    ///     Returns whether the decision SLA is breached at the given moment.
    /// </summary>
    public bool IsDecisionOverdueAt(DateTimeOffset moment) => Decision is null && moment > DecisionDue;

    private void Record(AuditAction action, Actor actor, string? details, DateTimeOffset occurredAt)
    {
        _auditTrail.Add(AuditEntry.Create(action, actor, details, occurredAt));
    }

    private void EnsureParty(Party party)
    {
        if (!Involves(party))
        {
            throw new PartyMismatchException("Only a party of the incident can perform this action.");
        }
    }

    private void EnsureStatusIn(string action, params ReadOnlySpan<IncidentStatus> allowedStatuses)
    {
        foreach (var allowedStatus in allowedStatuses)
        {
            if (Status == allowedStatus)
            {
                return;
            }
        }

        throw new InvalidIncidentStatusException(Status, action);
    }
}