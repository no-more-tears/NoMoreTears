using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     A reviewer rejected the qualification of the master in a category.
/// </summary>
public sealed record MasterCategoryRejectedEvent(
    Guid MasterId,
    Guid CategoryId,
    Guid ReviewerId,
    string Reason,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
