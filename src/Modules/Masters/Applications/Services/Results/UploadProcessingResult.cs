using NoMoreTears.Modules.Masters.Contracts.Responses;

namespace NoMoreTears.Modules.Masters.Applications.Services.Results;

/// <summary>
///     Outcome of the check of an uploaded file.
/// </summary>
public sealed record UploadProcessingResult(
    FileUploadResponse Upload,
    Guid MasterId,
    MasterResponse? Master,
    IReadOnlyList<StoredFileResponse> ObsoleteFiles);