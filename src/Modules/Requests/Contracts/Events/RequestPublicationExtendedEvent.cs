using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     The client chose to extend an expired request, it waits for acceptance again (FR-REQ-03).
/// </summary>
public sealed record RequestPublicationExtendedEvent(
    Guid RequestId,
    Guid ClientId,
    Guid CategoryId,
    DateTimeOffset AcceptanceDeadline,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);