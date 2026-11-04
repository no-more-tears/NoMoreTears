using NoMoreTears.Modules.Requests.Domain.Entities;
using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Modules.Requests.Domain.Exceptions;
using NoMoreTears.Modules.Requests.Domain.ValueObjects;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Aggregates;

/// <summary>
///     Client's request for a master.
/// </summary>
public sealed class Request : AuditableEntity<Guid>
{
    /// <summary>
    ///     Identifier of the client who owns the request.
    /// </summary>
    public Guid ClientId { get; }

    /// <summary>
    ///     Identifier of the service category from the Masters catalog.
    /// </summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    ///     Geocoded address of the work.
    /// </summary>
    public Address Address { get; private set; }

    /// <summary>
    ///     Description of the problem.
    /// </summary>
    public Description Description { get; private set; }

    /// <summary>
    ///     Current status.
    /// </summary>
    public RequestStatus Status { get; private set; }

    /// <summary>
    ///     Number of publications: 1 after the first publication, increased by each republication.
    /// </summary>
    public int PublicationCount { get; private set; }

    /// <summary>
    ///     Moment of the latest publication.
    /// </summary>
    public DateTimeOffset? PublishedAt { get; private set; }

    /// <summary>
    ///     Moment after which a published request becomes expired.
    /// </summary>
    public DateTimeOffset? AcceptanceDeadline { get; private set; }

    /// <summary>
    ///     Master who accepted the current publication.
    /// </summary>
    public Assignment? Assignment { get; private set; }

    /// <summary>
    ///     Moment after which a request marked as done is completed automatically.
    /// </summary>
    public DateTimeOffset? ConfirmationDeadline { get; private set; }

    /// <summary>
    ///     Moment of completion.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    ///     Failure of the current publication.
    /// </summary>
    public Failure? Failure { get; private set; }

    /// <summary>
    ///     Moment of cancellation.
    /// </summary>
    public DateTimeOffset? CancelledAt { get; private set; }

    /// <summary>
    ///     Reason of the cancellation, required after acceptance.
    /// </summary>
    public Reason? CancellationReason { get; private set; }

    /// <summary>
    ///     Identifier of the open Trust incident while the request is disputed.
    /// </summary>
    public Guid? IncidentId { get; private set; }

    /// <summary>
    ///     Status to which the request returns when the dispute is settled.
    /// </summary>
    public RequestStatus? StatusBeforeDispute { get; private set; }

    private readonly List<MediaFile> _mediaFiles = [];

    /// <summary>
    ///     Photos attached to the request.
    /// </summary>
    public IReadOnlyCollection<MediaFile> MediaFiles => _mediaFiles.AsReadOnly();

    private readonly List<StatusChange> _statusHistory = [];

    /// <summary>
    ///     History of all status changes.
    /// </summary>
    public IReadOnlyCollection<StatusChange> StatusHistory => _statusHistory.AsReadOnly();

    /// <summary>
    ///     Whether the request was published again after a failure, which raises its scoring priority.
    /// </summary>
    public bool IsRepublished => PublicationCount > 1;

    private Request(
        Guid id,
        Guid clientId,
        Guid categoryId,
        Address address,
        Description description)
    {
        Id = id;
        ClientId = clientId;
        CategoryId = categoryId;
        Address = address;
        Description = description;
        Status = RequestStatus.Draft;
    }

    /// <summary>
    ///     Creates a draft request.
    /// </summary>
    public static Request Create(
        Guid clientId,
        Guid categoryId,
        Address address,
        Description description,
        DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(description);
        EnsureNotEmpty(clientId, nameof(clientId));
        EnsureNotEmpty(categoryId, nameof(categoryId));

        var request = new Request(Guid.CreateVersion7(createdAt), clientId, categoryId, address, description);

        request.MarkAsCreated(createdAt);
        request._statusHistory.Add(StatusChange.Create(
            null,
            RequestStatus.Draft,
            StatusTransition.Created,
            Initiator.Of(InitiatorRole.Client, clientId),
            null,
            createdAt));

        return request;
    }

    /// <summary>
    ///     Changes the service category of a draft.
    /// </summary>
    public void ChangeCategory(
        Guid clientId,
        Guid categoryId,
        DateTimeOffset changedAt)
    {
        EnsureClient(clientId);
        EnsureDraft();
        EnsureNotEmpty(categoryId, nameof(categoryId));

        CategoryId = categoryId;
        MarkAsUpdated(changedAt);
    }

    /// <summary>
    ///     Changes the address of a draft.
    /// </summary>
    public void ChangeAddress(
        Guid clientId,
        Address address,
        DateTimeOffset changedAt)
    {
        ArgumentNullException.ThrowIfNull(address);
        EnsureClient(clientId);
        EnsureDraft();

        Address = address;
        MarkAsUpdated(changedAt);
    }

    /// <summary>
    ///     Changes the description of a draft.
    /// </summary>
    public void ChangeDescription(
        Guid clientId,
        Description description,
        DateTimeOffset changedAt)
    {
        ArgumentNullException.ThrowIfNull(description);
        EnsureClient(clientId);
        EnsureDraft();

        Description = description;
        MarkAsUpdated(changedAt);
    }

    /// <summary>
    ///     Attaches a photo to a draft.
    /// </summary>
    public MediaFile AttachMediaFile(
        Guid clientId,
        string storageKey,
        string contentType,
        long sizeInBytes,
        DateTimeOffset attachedAt)
    {
        EnsureClient(clientId);
        EnsureDraft();

        if (_mediaFiles.Count >= RequestRules.MaxMediaFiles)
        {
            throw new MediaFileLimitExceededException(RequestRules.MaxMediaFiles);
        }

        var mediaFile = MediaFile.Create(storageKey, contentType, sizeInBytes, attachedAt);

        if (_mediaFiles.Exists(attached => string.Equals(attached.StorageKey, mediaFile.StorageKey, StringComparison.Ordinal)))
        {
            throw new InvalidMediaFileException($"Media file '{mediaFile.StorageKey}' is already attached.");
        }

        _mediaFiles.Add(mediaFile);
        MarkAsUpdated(attachedAt);

        return mediaFile;
    }

    /// <summary>
    ///     Detaches a photo from a draft.
    /// </summary>
    public void DetachMediaFile(
        Guid clientId,
        Guid mediaFileId,
        DateTimeOffset detachedAt)
    {
        EnsureClient(clientId);
        EnsureDraft();

        var mediaFile = _mediaFiles.Find(attached => attached.Id == mediaFileId)
            ?? throw new MediaFileNotFoundException(mediaFileId);

        _mediaFiles.Remove(mediaFile);
        MarkAsUpdated(detachedAt);
    }

    /// <summary>
    ///     Discards a draft. Published requests cannot be discarded, only cancelled.
    /// </summary>
    public void Discard(
        Guid clientId,
        DateTimeOffset discardedAt)
    {
        EnsureClient(clientId);
        EnsureStatusIn("discard", RequestStatus.Draft);

        MarkAsDeleted(discardedAt);
    }

    /// <summary>
    ///     Publishes a draft. From this moment the content is fixed.
    /// </summary>
    public void Publish(
        Guid clientId,
        PublicationPolicy policy,
        int publishedRequestsCount,
        DateTimeOffset publishedAt)
    {
        ArgumentNullException.ThrowIfNull(policy);
        EnsureClient(clientId);
        EnsureStatusIn("publish", RequestStatus.Draft);
        policy.EnsureAllowsPublication(publishedRequestsCount);

        PublicationCount++;
        PublishedAt = publishedAt;
        AcceptanceDeadline = policy.AcceptanceDeadlineFrom(publishedAt);
        ChangeStatus(RequestStatus.Published, StatusTransition.Published, ClientInitiator, null, publishedAt);
    }

    /// <summary>
    ///     Marks a published request as expired when its acceptance deadline has passed.
    /// </summary>
    public void Expire(DateTimeOffset expiredAt)
    {
        EnsureStatusIn("expire", RequestStatus.Published);

        if (expiredAt < AcceptanceDeadline)
        {
            throw new RequestDeadlineNotReachedException(
                $"The request can expire only after {AcceptanceDeadline:O}.");
        }

        ChangeStatus(RequestStatus.Expired, StatusTransition.Expired, Initiator.System, null, expiredAt);
    }

    /// <summary>
    ///     Extends the waiting for acceptance of an expired request.
    /// </summary>
    public void Extend(
        Guid clientId,
        PublicationPolicy policy,
        int publishedRequestsCount,
        DateTimeOffset extendedAt)
    {
        ArgumentNullException.ThrowIfNull(policy);
        EnsureClient(clientId);
        EnsureStatusIn("extend", RequestStatus.Expired);
        policy.EnsureAllowsPublication(publishedRequestsCount);

        AcceptanceDeadline = policy.AcceptanceDeadlineFrom(extendedAt);
        ChangeStatus(RequestStatus.Published, StatusTransition.Extended, ClientInitiator, null, extendedAt);
    }

    /// <summary>
    ///     Accepts a published request. Exactly one master can hold the request at a time.
    /// </summary>
    public void Accept(
        Guid masterId,
        TimeSpan arrivalInterval,
        DateTimeOffset acceptedAt)
    {
        EnsureStatusIn("accept", RequestStatus.Published);

        if (acceptedAt >= AcceptanceDeadline)
        {
            throw new AcceptanceDeadlinePassedException();
        }

        if (masterId == ClientId)
        {
            throw new RequestParticipantMismatchException("A client cannot accept their own request.");
        }

        Assignment = Assignment.Of(masterId, acceptedAt, arrivalInterval);
        ChangeStatus(RequestStatus.InProgress, StatusTransition.Accepted, MasterInitiator, null, acceptedAt);
    }

    /// <summary>
    ///     Marks the work as done by the assigned master and starts the confirmation period.
    /// </summary>
    public void MarkAsDone(
        Guid masterId,
        DateTimeOffset doneAt)
    {
        EnsureAssignedMaster(masterId);
        EnsureStatusIn("mark as done", RequestStatus.InProgress);

        ConfirmationDeadline = doneAt + RequestRules.CompletionConfirmationPeriod;
        ChangeStatus(RequestStatus.AwaitingConfirmation, StatusTransition.MarkedAsDone, MasterInitiator, null, doneAt);
    }

    /// <summary>
    ///     Confirms the completion by the client.
    /// </summary>
    public void ConfirmCompletion(
        Guid clientId,
        DateTimeOffset confirmedAt)
    {
        EnsureClient(clientId);
        EnsureStatusIn("confirm completion of", RequestStatus.AwaitingConfirmation);

        Complete(StatusTransition.CompletionConfirmed, ClientInitiator, confirmedAt);
    }

    /// <summary>
    ///     Completes the request automatically when the client neither confirmed nor objected within the period.
    /// </summary>
    public void CompleteAutomatically(DateTimeOffset completedAt)
    {
        EnsureStatusIn("complete automatically", RequestStatus.AwaitingConfirmation);

        if (completedAt < ConfirmationDeadline)
        {
            throw new RequestDeadlineNotReachedException(
                $"The request can be completed automatically only after {ConfirmationDeadline:O}.");
        }

        Complete(StatusTransition.CompletedAutomatically, Initiator.System, completedAt);
    }

    /// <summary>
    ///     Objects to the completion marked by the master and returns the request to execution.
    /// </summary>
    public void ObjectToCompletion(
        Guid clientId,
        Reason reason,
        DateTimeOffset objectedAt)
    {
        ArgumentNullException.ThrowIfNull(reason);
        EnsureClient(clientId);
        EnsureStatusIn("object to completion of", RequestStatus.AwaitingConfirmation);

        ConfirmationDeadline = null;
        ChangeStatus(RequestStatus.InProgress, StatusTransition.CompletionObjected, ClientInitiator, reason, objectedAt);
    }

    /// <summary>
    ///     Reports that the master did not arrive within the expected time plus the buffer.
    /// </summary>
    public void ReportNoShow(
        Guid clientId,
        DateTimeOffset reportedAt)
    {
        EnsureClient(clientId);
        EnsureStatusIn("report a no-show for", RequestStatus.InProgress);

        var assignment = Assignment!;

        if (!assignment.IsArrivalOverdueAt(reportedAt))
        {
            throw new RequestDeadlineNotReachedException(
                $"A no-show can be reported only after {assignment.NoShowReportableFrom:O}.");
        }

        Failure = Failure.Of(FailureKind.NoShow, assignment.MasterId, null);
        ChangeStatus(RequestStatus.Failed, StatusTransition.NoShowReported, ClientInitiator, null, reportedAt);
    }

    /// <summary>
    ///     Records the refusal of the assigned master to do the work.
    /// </summary>
    public void Refuse(
        Guid masterId,
        Reason reason,
        DateTimeOffset refusedAt)
    {
        ArgumentNullException.ThrowIfNull(reason);
        EnsureAssignedMaster(masterId);
        EnsureStatusIn("refuse", RequestStatus.InProgress);

        Failure = Failure.Of(FailureKind.MasterRefusal, masterId, reason);
        ChangeStatus(RequestStatus.Failed, StatusTransition.RefusedByMaster, MasterInitiator, reason, refusedAt);
    }

    /// <summary>
    ///     Publishes a failed request again with a raised scoring priority.
    /// </summary>
    public void Republish(
        Guid clientId,
        PublicationPolicy policy,
        int publishedRequestsCount,
        DateTimeOffset republishedAt)
    {
        ArgumentNullException.ThrowIfNull(policy);
        EnsureClient(clientId);
        EnsureStatusIn("republish", RequestStatus.Failed);
        policy.EnsureAllowsPublication(publishedRequestsCount);

        PublicationCount++;
        PublishedAt = republishedAt;
        AcceptanceDeadline = policy.AcceptanceDeadlineFrom(republishedAt);
        Assignment = null;
        ConfirmationDeadline = null;
        Failure = null;
        ChangeStatus(RequestStatus.Published, StatusTransition.Republished, ClientInitiator, null, republishedAt);
    }

    /// <summary>
    ///     Cancels a request that has not been accepted yet, without consequences.
    /// </summary>
    public void Cancel(
        Guid clientId,
        DateTimeOffset cancelledAt)
    {
        EnsureClient(clientId);

        if (Status is RequestStatus.InProgress)
        {
            throw new CancellationReasonRequiredException();
        }

        EnsureStatusIn("cancel", RequestStatus.Published, RequestStatus.Expired);

        CancelledAt = cancelledAt;
        ChangeStatus(RequestStatus.Cancelled, StatusTransition.CancelledBeforeAcceptance, ClientInitiator, null, cancelledAt);
    }

    /// <summary>
    ///     Cancels a request with a reason. After acceptance the cancellation is counted in abuse signals.
    /// </summary>
    public void Cancel(
        Guid clientId,
        Reason reason,
        DateTimeOffset cancelledAt)
    {
        ArgumentNullException.ThrowIfNull(reason);
        EnsureClient(clientId);

        var transition = Status switch
        {
            RequestStatus.Published or RequestStatus.Expired => StatusTransition.CancelledBeforeAcceptance,
            RequestStatus.InProgress => StatusTransition.CancelledAfterAcceptance,
            _ => throw new InvalidRequestStatusTransitionException(Status, "cancel")
        };

        CancelledAt = cancelledAt;
        CancellationReason = reason;
        ChangeStatus(RequestStatus.Cancelled, transition, ClientInitiator, reason, cancelledAt);
    }

    /// <summary>
    ///     Moves the request to dispute after Trust accepted a complaint.
    ///     Only one incident can be open at a time.
    /// </summary>
    public void Dispute(
        Guid incidentId,
        DateTimeOffset disputedAt)
    {
        EnsureNotEmpty(incidentId, nameof(incidentId));
        EnsureStatusIn("dispute", RequestStatus.InProgress, RequestStatus.AwaitingConfirmation, RequestStatus.Completed, RequestStatus.Failed);

        IncidentId = incidentId;
        StatusBeforeDispute = Status;
        ChangeStatus(RequestStatus.Disputed, StatusTransition.Disputed, Initiator.System, null, disputedAt);
    }

    /// <summary>
    ///     Returns the request to the status it had before the dispute after Trust closed the incident.
    /// </summary>
    public void SettleDispute(
        Guid incidentId,
        DateTimeOffset settledAt)
    {
        EnsureStatusIn("settle the dispute of", RequestStatus.Disputed);

        if (IncidentId != incidentId)
        {
            throw new IncidentMismatchException(incidentId);
        }

        var previousStatus = StatusBeforeDispute!.Value;
        IncidentId = null;
        StatusBeforeDispute = null;
        ChangeStatus(previousStatus, StatusTransition.DisputeSettled, Initiator.System, null, settledAt);
    }

    /// <summary>
    ///     Returns whether the client's address and phone may be revealed to the given master.
    /// </summary>
    public bool CanRevealContactsTo(Guid masterId)
    {
        return Assignment is not null
            && Assignment.IsAssignedTo(masterId)
            && (Status is RequestStatus.InProgress
                or RequestStatus.AwaitingConfirmation
                or RequestStatus.Completed
                or RequestStatus.Disputed);
    }

    private Initiator ClientInitiator => Initiator.Of(InitiatorRole.Client, ClientId);

    private Initiator MasterInitiator => Initiator.Of(InitiatorRole.Master, Assignment!.MasterId);

    private void Complete(
        StatusTransition transition,
        Initiator initiator,
        DateTimeOffset completedAt)
    {
        CompletedAt = completedAt;
        ConfirmationDeadline = null;
        ChangeStatus(RequestStatus.Completed, transition, initiator, null, completedAt);
    }

    private void ChangeStatus(
        RequestStatus status,
        StatusTransition transition,
        Initiator initiator,
        Reason? reason,
        DateTimeOffset changedAt)
    {
        _statusHistory.Add(StatusChange.Create(Status, status, transition, initiator, reason, changedAt));
        Status = status;
        MarkAsUpdated(changedAt);
    }

    private void EnsureStatusIn(
        string action,
        params ReadOnlySpan<RequestStatus> allowedStatuses)
    {
        foreach (var allowedStatus in allowedStatuses)
        {
            if (Status == allowedStatus)
            {
                return;
            }
        }

        throw new InvalidRequestStatusTransitionException(Status, action);
    }

    private void EnsureDraft()
    {
        if (Status is not RequestStatus.Draft)
        {
            throw new RequestContentLockedException(Status);
        }
    }

    private void EnsureClient(Guid clientId)
    {
        if (clientId != ClientId)
        {
            throw new RequestParticipantMismatchException("Only the client of the request can perform this action.");
        }
    }

    private void EnsureAssignedMaster(Guid masterId)
    {
        if (Assignment is null || !Assignment.IsAssignedTo(masterId))
        {
            throw new RequestParticipantMismatchException("Only the assigned master can perform this action.");
        }
    }

    private static void EnsureNotEmpty(Guid identifier, string name)
    {
        if (identifier == Guid.Empty)
        {
            throw new InvalidIdentifierException(name);
        }
    }
}