using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Trust.Contracts.Events;

/// <summary>
///     A sanction was lifted, for example after an accepted appeal.
/// </summary>
public sealed record SanctionLiftedEvent(
    Guid SanctionId,
    Guid SubjectId,
    string SubjectRole,
    string Kind,
    DateTimeOffset LiftedAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
