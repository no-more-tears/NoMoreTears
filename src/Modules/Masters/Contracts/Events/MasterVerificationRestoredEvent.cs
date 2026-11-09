using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     A revocation was lifted before the master applied again, so the verification is restored.
/// </summary>
public sealed record MasterVerificationRestoredEvent(
    Guid MasterId,
    Guid SanctionId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
