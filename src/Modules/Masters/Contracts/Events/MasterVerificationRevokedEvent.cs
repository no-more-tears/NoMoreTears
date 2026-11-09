using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     Verification was revoked by a sanction (step 4); the master can apply again from
///     <see cref="ReverificationAllowedFrom"/> with new documents.
/// </summary>
public sealed record MasterVerificationRevokedEvent(
    Guid MasterId,
    Guid SanctionId,
    DateTimeOffset RevokedAt,
    DateTimeOffset ReverificationAllowedFrom,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
