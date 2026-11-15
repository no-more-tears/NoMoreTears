namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DeleteObsoleteFiles;

/// <summary>
///     Removes stored files that are no longer referenced. Sent by the Requests consumers after the change is committed.
/// </summary>
public sealed record DeleteObsoleteFilesCommand(
    IReadOnlyList<StoredFileResponse> Files) : ICommand;