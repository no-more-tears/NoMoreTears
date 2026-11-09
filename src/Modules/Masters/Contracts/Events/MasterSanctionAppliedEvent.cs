using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     A sanction or temporary measure of Trust was applied to the master.
///     <see cref="Kind"/> is <c>Warning</c>, <c>Restriction</c>, <c>Suspension</c> or <c>Deverification</c>.
/// </summary>
public sealed record MasterSanctionAppliedEvent(
    Guid MasterId,
    Guid SanctionId,
    string Kind,
    DateTimeOffset ImposedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
