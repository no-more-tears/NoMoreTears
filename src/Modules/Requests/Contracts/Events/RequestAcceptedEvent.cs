using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     A master accepted a request; the address is revealed to the master and the client is notified about the expected arrival (FR-REQ-04, FR-REQ-10).
/// </summary>
public sealed record RequestAcceptedEvent(
    Guid RequestId,
    Guid ClientId,
    Guid MasterId,
    Guid CategoryId,
    DateTimeOffset AcceptedAt,
    DateTimeOffset ExpectedArrivalAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);