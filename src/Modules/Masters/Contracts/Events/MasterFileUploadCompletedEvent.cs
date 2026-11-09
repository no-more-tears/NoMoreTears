using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     An uploaded file passed the check and is attached to the profile.
///     <see cref="Purpose"/> is <c>Avatar</c>, <c>IdentityDocument</c> or <c>QualificationDocument</c>.
/// </summary>
public sealed record MasterFileUploadCompletedEvent(
    Guid MasterId,
    Guid UploadId,
    string Purpose,
    Guid? CategoryId,
    Guid? DocumentId,
    string FileName,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
