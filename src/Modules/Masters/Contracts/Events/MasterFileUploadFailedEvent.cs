using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     An uploaded file was rejected by the check or could not be attached to the profile.
/// </summary>
public sealed record MasterFileUploadFailedEvent(
    Guid MasterId,
    Guid UploadId,
    string Purpose,
    string FileName,
    string Reason,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
