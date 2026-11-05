using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Modules.Requests.Domain.ValueObjects;

namespace NoMoreTears.Modules.Requests.Domain.Entities;

/// <summary>
///     Immutable entry of the request status history.
/// </summary>
public sealed class StatusChange
{
    /// <summary>
    ///     Unique identifier of the entry.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     Status before the change.
    /// </summary>
    public RequestStatus? From { get; }

    /// <summary>
    ///     Status after the change.
    /// </summary>
    public RequestStatus To { get; }

    /// <summary>
    ///     Business basis of the change.
    /// </summary>
    public StatusTransition Transition { get; }

    /// <summary>
    ///     Party that initiated the change.
    /// </summary>
    public Initiator Initiator { get; }

    /// <summary>
    ///     Reason given by the initiator, if any.
    /// </summary>
    public Reason? Reason { get; }

    /// <summary>
    ///     Moment of the change.
    /// </summary>
    public DateTimeOffset OccurredAt { get; }

    private StatusChange(
        Guid id,
        RequestStatus? from,
        RequestStatus to,
        StatusTransition transition,
        Initiator initiator,
        Reason? reason,
        DateTimeOffset occurredAt)
    {
        Id = id;
        From = from;
        To = to;
        Transition = transition;
        Initiator = initiator;
        Reason = reason;
        OccurredAt = occurredAt;
    }

    /// <summary>
    ///     Creates a history entry.
    ///     Only the <see cref="Aggregates.Request"/> aggregate records its history.
    /// </summary>
    internal static StatusChange Create(
        RequestStatus? from,
        RequestStatus to,
        StatusTransition transition,
        Initiator initiator,
        Reason? reason,
        DateTimeOffset occurredAt)
    {
        return new StatusChange(Guid.CreateVersion7(occurredAt), from, to, transition, initiator, reason, occurredAt);
    }
}