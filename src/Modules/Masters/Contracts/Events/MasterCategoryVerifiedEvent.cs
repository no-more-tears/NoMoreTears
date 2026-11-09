using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     A reviewer confirmed the qualification of the master in a category.
/// </summary>
public sealed record MasterCategoryVerifiedEvent(
    Guid MasterId,
    Guid CategoryId,
    Guid ReviewerId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
