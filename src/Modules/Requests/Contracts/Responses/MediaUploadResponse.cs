namespace NoMoreTears.Modules.Requests.Contracts.Responses;

/// <summary>
///     Status of an uploaded photo; <see cref="Status"/> is <c>Pending</c>, <c>Completed</c> or <c>Failed</c>.
/// </summary>
public sealed record MediaUploadResponse(
    Guid Id,
    Guid RequestId,
    string FileName,
    string ContentType,
    long SizeInBytes,
    string Status,
    string? FailureReason,
    Guid? MediaFileId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? ProcessedAt);