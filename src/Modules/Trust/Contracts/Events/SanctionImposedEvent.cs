using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Trust.Contracts.Events;

/// <summary>
///     A sanction of the ladder was imposed on a party (FR-TRS-06). Masters applies sanctions of masters to the
///     feed access, Dispatch to the scoring.
/// </summary>
/// <param name="SanctionId">Identifier of the sanction; other modules keep it to lift the sanction later.</param>
/// <param name="SubjectId">User identifier of the sanctioned party (for a master it is also the master id).</param>
/// <param name="SubjectRole">One of <see cref="Constants.PartyRoles"/>.</param>
/// <param name="Kind">One of <see cref="Constants.SanctionKinds"/>.</param>
/// <param name="Step">Step of the ladder, from 1 to 4.</param>
public sealed record SanctionImposedEvent(
    Guid SanctionId,
    Guid SubjectId,
    string SubjectRole,
    string Kind,
    int Step,
    Guid? IncidentId,
    DateTimeOffset ImposedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
