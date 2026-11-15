using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     A photo passed the check and was attached to the draft.
/// </summary>
public sealed record RequestMediaUploadCompletedEvent(
    Guid ClientId,
    Guid RequestId,
    Guid UploadId,
    Guid MediaFileId,
    string FileName,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);