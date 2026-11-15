using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     Nobody accepted a published request before its deadline; the client chooses to extend or cancel it (FR-REQ-03).
/// </summary>
public sealed record RequestExpiredEvent(
    Guid RequestId,
    Guid ClientId,
    Guid CategoryId,
    DateTimeOffset AcceptanceDeadline,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);