namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ProcessFileUpload;

/// <summary>
///     Checks a staged file and attaches it to the profile. Sent by the Masters consumers, not exposed over HTTP.
/// </summary>
public sealed record ProcessFileUploadCommand(
    Guid UploadId) : ICommand;
