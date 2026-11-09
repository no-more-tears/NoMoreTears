using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     Internal event of the Masters module: a file is stored in the temporary bucket and waits for the check
///     by the Masters consumers. Other modules should not depend on it.
/// </summary>
public sealed record MasterFileStagedEvent(
    Guid UploadId,
    Guid MasterId,
    string Purpose,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
