using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     A category was archived: it can no longer be chosen for new requests or profiles.
/// </summary>
public sealed record CategoryArchivedEvent(
    Guid CategoryId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
