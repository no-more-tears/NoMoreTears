using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     Internal event of the Requests module: a photo is stored in the temporary bucket and waits for the check by the Requests consumers. Other modules should not depend on it.
/// </summary>
public sealed record RequestMediaStagedEvent(
    Guid UploadId,
    Guid RequestId,
    Guid ClientId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);