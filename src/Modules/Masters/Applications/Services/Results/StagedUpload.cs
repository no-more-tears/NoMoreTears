using NoMoreTears.Modules.Masters.Contracts.Responses;

namespace NoMoreTears.Modules.Masters.Applications.Services.Results;

/// <summary>
///     A file stored in the temporary bucket and registered for the check.
/// </summary>
public sealed record StagedUpload(
    FileUploadResponse Upload,
    Guid MasterId);