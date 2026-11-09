using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The master removed a category from the profile.
/// </summary>
public sealed record MasterCategoryRemovedEvent(
    Guid MasterId,
    Guid CategoryId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
