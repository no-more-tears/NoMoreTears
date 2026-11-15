namespace NoMoreTears.Modules.Requests.Contracts.Responses;

/// <summary>
///     Entry of the status history: moment, initiator and ground of a transition.
///     <see cref="InitiatorRole"/> is <c>System</c>, <c>Client</c> or <c>Master</c>.
/// </summary>
public sealed record StatusChangeResponse(
    Guid Id,
    string? From,
    string To,
    string Transition,
    string InitiatorRole,
    Guid? InitiatorId,
    string? Reason,
    DateTimeOffset OccurredAt);