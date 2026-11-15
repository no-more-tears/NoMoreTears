using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Events;

/// <summary>
///     Builds and publishes integration events of the Requests module from the state returned by services.
/// </summary>
internal static class RequestsEvents
{
    /// <summary>
    ///     Moment of the latest change of the request.
    /// </summary>
    public static DateTimeOffset ChangedAt(RequestResponse request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return request.LastUpdatedAt ?? request.CreatedAt
            ?? throw new InvalidOperationException($"Request {request.Id} has no change time.");
    }

    /// <summary>
    ///     Master of the current publication: the assigned one or the one who failed it.
    /// </summary>
    public static Guid? MasterOf(RequestResponse request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return request.Assignment?.MasterId ?? request.Failure?.MasterId;
    }

    /// <summary>
    ///     The latest transition of the request (FR-REQ-08).
    /// </summary>
    public static RequestStatusChangedEvent StatusChanged(RequestResponse request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var change = request.LastStatusChange;

        return new RequestStatusChangedEvent(
            request.Id,
            request.ClientId,
            MasterOf(request),
            request.CategoryId,
            change.From,
            change.To,
            change.Transition,
            change.InitiatorRole,
            change.InitiatorId,
            change.Reason,
            change.OccurredAt);
    }

    public static RequestDraftUpdatedEvent DraftUpdated(RequestResponse request) =>
        new(request.Id, request.ClientId, request.CategoryId, request.MediaFiles.Count, ChangedAt(request));

    public static RequestPublishedEvent Published(RequestResponse request) =>
        new(
            request.Id,
            request.ClientId,
            request.CategoryId,
            request.Address!.Coordinates.Latitude,
            request.Address.Coordinates.Longitude,
            request.PublicationCount,
            request.IsRepublished,
            request.PublishedAt!.Value,
            request.AcceptanceDeadline!.Value,
            ChangedAt(request));

    public static RequestExpiredEvent Expired(RequestResponse request) =>
        new(request.Id, request.ClientId, request.CategoryId, request.AcceptanceDeadline!.Value, ChangedAt(request));

    public static RequestCompletedEvent Completed(RequestResponse request) =>
        new(
            request.Id,
            request.ClientId,
            request.Assignment!.MasterId,
            request.CategoryId,
            request.LastStatusChange.Transition == nameof(Domain.Enums.StatusTransition.CompletionConfirmed),
            request.CompletedAt!.Value,
            ChangedAt(request));

    public static RequestFailedEvent Failed(RequestResponse request) =>
        new(
            request.Id,
            request.ClientId,
            request.Failure!.MasterId,
            request.Failure.Kind,
            request.Failure.Reason,
            ChangedAt(request));

    /// <summary>
    ///     Publishes the event of a transition followed by <see cref="RequestStatusChangedEvent"/>.
    ///     The event type stays concrete, so consumers of <typeparamref name="TEvent"/> receive it.
    /// </summary>
    public static async Task PublishTransitionAsync<TEvent>(
        this IEventPublisher<IRequestsOutbox> eventPublisher,
        TEvent @event,
        RequestResponse request,
        CancellationToken cancellationToken)
        where TEvent : class, IEvent
    {
        ArgumentNullException.ThrowIfNull(eventPublisher);

        await eventPublisher.PublishAsync(@event, cancellationToken);
        await eventPublisher.PublishAsync(StatusChanged(request), cancellationToken);
    }

    /// <summary>
    ///     Publishes the removal of stored files when there are any.
    /// </summary>
    public static async Task PublishObsoleteFilesAsync(
        this IEventPublisher<IRequestsOutbox> eventPublisher,
        Guid requestId,
        IReadOnlyList<StoredFileResponse> files,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(eventPublisher);
        ArgumentNullException.ThrowIfNull(files);

        if (files.Count > 0)
        {
            await eventPublisher.PublishAsync(new RequestFilesObsoletedEvent(requestId, files, occurredAt), cancellationToken);
        }
    }
}