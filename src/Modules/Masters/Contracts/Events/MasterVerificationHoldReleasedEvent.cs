using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The verification hold was released after a human check.
/// </summary>
public sealed record MasterVerificationHoldReleasedEvent(
    Guid MasterId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
