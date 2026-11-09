using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UploadAvatar;

/// <summary>
///     Receives a profile photo; it is checked and attached asynchronously.
/// </summary>
public sealed record UploadAvatarCommand(
    Guid MasterId,
    Stream Content,
    string FileName,
    string ContentType,
    long SizeInBytes) : ICommand<FileUploadResponse>, IMasterOwnedRequest;
