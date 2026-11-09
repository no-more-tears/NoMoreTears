namespace NoMoreTears.Modules.Masters.Contracts.Responses;

/// <summary>
///     Location of a stored file.
/// </summary>
public sealed record StoredFileResponse(
    string Bucket,
    string Key);
