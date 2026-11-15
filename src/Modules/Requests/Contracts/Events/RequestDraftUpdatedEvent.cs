using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     The client changed the category, address, description or photos of a draft.
/// </summary>
public sealed record RequestDraftUpdatedEvent(
    Guid RequestId,
    Guid ClientId,
    Guid CategoryId,
    int MediaFilesCount,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);