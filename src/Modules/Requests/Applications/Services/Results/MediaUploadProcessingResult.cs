using NoMoreTears.Modules.Requests.Contracts.Responses;

namespace NoMoreTears.Modules.Requests.Applications.Services.Results;

/// <summary>
///     Outcome of the check of an uploaded photo; <see cref="Request"/> is <c>null</c> when the photo was rejected.
/// </summary>
public sealed record MediaUploadProcessingResult(
    MediaUploadResponse Upload,
    Guid ClientId,
    RequestResponse? Request,
    IReadOnlyList<StoredFileResponse> ObsoleteFiles);