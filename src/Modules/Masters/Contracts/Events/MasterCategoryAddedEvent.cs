using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The master added a category; it awaits review (<c>Pending</c>).
/// </summary>
public sealed record MasterCategoryAddedEvent(
    Guid MasterId,
    Guid CategoryId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
