using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     Every transition of the request state machine with its initiator and ground (FR-REQ-08). A conformist snapshot for Trust; <see cref="MasterId"/> is the master of the current publication.
/// </summary>
public sealed record RequestStatusChangedEvent(
    Guid RequestId,
    Guid ClientId,
    Guid? MasterId,
    Guid CategoryId,
    string? From,
    string To,
    string Transition,
    string InitiatorRole,
    Guid? InitiatorId,
    string? Reason,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);