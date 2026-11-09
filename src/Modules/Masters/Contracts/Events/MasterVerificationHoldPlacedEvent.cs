using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     Verification is blocked until a human check, for example after a suspected duplicate account.
/// </summary>
public sealed record MasterVerificationHoldPlacedEvent(
    Guid MasterId,
    string Reason,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
