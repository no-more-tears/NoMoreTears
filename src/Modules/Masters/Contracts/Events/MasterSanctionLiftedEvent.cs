using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     A sanction or temporary measure was lifted, for example after an accepted appeal.
/// </summary>
public sealed record MasterSanctionLiftedEvent(
    Guid MasterId,
    Guid SanctionId,
    string Kind,
    DateTimeOffset LiftedAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
