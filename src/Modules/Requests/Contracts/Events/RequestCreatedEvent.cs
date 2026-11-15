using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     A client created a draft request (FR-REQ-01). <see cref="SourceRequestId"/> is set when the draft repeats an earlier request (FR-REQ-11).
/// </summary>
public sealed record RequestCreatedEvent(
    Guid RequestId,
    Guid ClientId,
    Guid CategoryId,
    Guid? SourceRequestId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);