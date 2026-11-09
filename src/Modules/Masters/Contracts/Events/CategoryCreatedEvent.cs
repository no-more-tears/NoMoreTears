using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     A category was added to the catalog, the single source of truth for Requests and Dispatch.
/// </summary>
public sealed record CategoryCreatedEvent(
    Guid CategoryId,
    string Name,
    string? Description,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
