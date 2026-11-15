using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     A request was completed, confirmed by the client or automatically (FR-REQ-06). Trust opens the review window.
/// </summary>
public sealed record RequestCompletedEvent(
    Guid RequestId,
    Guid ClientId,
    Guid MasterId,
    Guid CategoryId,
    bool IsConfirmedByClient,
    DateTimeOffset CompletedAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);