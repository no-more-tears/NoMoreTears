using NoMoreTears.Modules.Requests.Contracts.Responses;

namespace NoMoreTears.Modules.Requests.Applications.Services.Results;

/// <summary>
///     Request after stored files were detached from it, with the files to remove after the commit.
/// </summary>
public sealed record RequestFileRemoval(
    RequestResponse Request,
    IReadOnlyList<StoredFileResponse> RemovedFiles);