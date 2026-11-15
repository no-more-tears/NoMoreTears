using NoMoreTears.Modules.Requests.Contracts.Responses;

namespace NoMoreTears.Modules.Requests.Applications.Services.Results;

/// <summary>
///     A photo stored in the temporary bucket and registered for the check.
/// </summary>
public sealed record StagedMediaUpload(
    MediaUploadResponse Upload,
    Guid ClientId);