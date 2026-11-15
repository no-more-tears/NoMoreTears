using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     A photo was rejected by the check or could not be attached, for example because the request was published meanwhile.
/// </summary>
public sealed record RequestMediaUploadFailedEvent(
    Guid ClientId,
    Guid RequestId,
    Guid UploadId,
    string FileName,
    string Reason,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);