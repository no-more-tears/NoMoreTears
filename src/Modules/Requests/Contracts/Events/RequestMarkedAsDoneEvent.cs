using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     The master marked the work as done; the request completes when the client confirms or automatically at <see cref="ConfirmationDeadline"/> (FR-REQ-06).
/// </summary>
public sealed record RequestMarkedAsDoneEvent(
    Guid RequestId,
    Guid ClientId,
    Guid MasterId,
    DateTimeOffset ConfirmationDeadline,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);