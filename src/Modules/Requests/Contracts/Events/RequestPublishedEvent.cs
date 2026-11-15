using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     A request was published and waits for acceptance; Dispatch routes it to the feeds of masters. A republication after a failure has <see cref="IsRepublished"/> set and a raised scoring priority (FR-REQ-07).
/// </summary>
public sealed record RequestPublishedEvent(
    Guid RequestId,
    Guid ClientId,
    Guid CategoryId,
    double Latitude,
    double Longitude,
    int PublicationNumber,
    bool IsRepublished,
    DateTimeOffset PublishedAt,
    DateTimeOffset AcceptanceDeadline,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);