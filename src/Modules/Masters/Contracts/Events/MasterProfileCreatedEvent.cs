using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     A user created a master profile. The profile is not verified yet, so the user has no Master role.
/// </summary>
public sealed record MasterProfileCreatedEvent(
    Guid MasterId,
    string FirstName,
    string LastName,
    IReadOnlyList<Guid> CategoryIds,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
