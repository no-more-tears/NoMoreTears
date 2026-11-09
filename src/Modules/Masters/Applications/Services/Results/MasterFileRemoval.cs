using NoMoreTears.Modules.Masters.Contracts.Responses;

namespace NoMoreTears.Modules.Masters.Applications.Services.Results;

/// <summary>
///     Profile after a stored file was detached from it, with the file to remove after the commit.
/// </summary>
public sealed record MasterFileRemoval(
    MasterResponse Master,
    StoredFileResponse RemovedFile);