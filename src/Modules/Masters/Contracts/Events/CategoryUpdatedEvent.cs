using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The name or description of a category changed.
/// </summary>
public sealed record CategoryUpdatedEvent(
    Guid CategoryId,
    string Name,
    string? Description,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
