using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The profile is verified: the master gets access to the feed in the verified categories and area.
/// </summary>
public sealed record MasterVerifiedEvent(
    Guid MasterId,
    Guid ReviewerId,
    IReadOnlyList<Guid> VerifiedCategoryIds,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
