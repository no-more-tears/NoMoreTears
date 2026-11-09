using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The profile was rejected with a reason; the master can fix it and submit it again.
/// </summary>
public sealed record MasterVerificationRejectedEvent(
    Guid MasterId,
    Guid ReviewerId,
    string Reason,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
