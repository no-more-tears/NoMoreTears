namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ProcessMediaUpload;

/// <summary>
///     Checks a staged photo and attaches it to the draft. Sent by the Requests consumers, not exposed over HTTP.
/// </summary>
public sealed record ProcessMediaUploadCommand(
    Guid UploadId) : ICommand;