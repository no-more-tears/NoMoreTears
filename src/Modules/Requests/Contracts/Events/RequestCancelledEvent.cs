using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     The client cancelled a request (FR-REQ-05). A cancellation after acceptance has a reason and is counted in abuse signals.
/// </summary>
public sealed record RequestCancelledEvent(
    Guid RequestId,
    Guid ClientId,
    Guid? MasterId,
    bool IsAfterAcceptance,
    string? Reason,
    DateTimeOffset CancelledAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);