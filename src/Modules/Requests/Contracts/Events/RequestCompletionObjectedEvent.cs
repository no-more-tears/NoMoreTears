using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     The client objected to the completion marked by the master; the request returns to execution.
/// </summary>
public sealed record RequestCompletionObjectedEvent(
    Guid RequestId,
    Guid ClientId,
    Guid MasterId,
    string Reason,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);