namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.DeleteObsoleteFiles;

/// <summary>
///     Removes stored files that are no longer referenced. Sent by the Masters consumers after the change is committed.
/// </summary>
public sealed record DeleteObsoleteFilesCommand(
    IReadOnlyList<StoredFileResponse> Files) : ICommand;
