using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The profile photo was set, replaced or removed.
/// </summary>
public sealed record MasterAvatarChangedEvent(
    Guid MasterId,
    bool HasAvatar,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
