using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     A user with the Worker role created a master profile. The profile is not verified yet.
/// </summary>
public sealed record MasterProfileCreatedEvent(
    Guid MasterId,
    string FirstName,
    string LastName,
    IReadOnlyList<Guid> CategoryIds,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
