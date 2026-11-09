namespace NoMoreTears.Modules.Masters.Contracts.Responses;

/// <summary>
///     Status of an uploaded file. <see cref="Purpose"/> is <c>Avatar</c>, <c>IdentityDocument</c> or
///     <c>QualificationDocument</c>; <see cref="Status"/> is <c>Pending</c>, <c>Completed</c> or <c>Failed</c>.
/// </summary>
public sealed record FileUploadResponse(
    Guid Id,
    string Purpose,
    Guid? CategoryId,
    string FileName,
    string ContentType,
    long SizeInBytes,
    string Status,
    string? FailureReason,
    Guid? DocumentId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? ProcessedAt);
