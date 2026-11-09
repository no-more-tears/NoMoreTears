using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UploadDocument;

/// <summary>
///     Receives a verification document; it is checked and attached asynchronously.
/// </summary>
public sealed record UploadDocumentCommand(
    Guid MasterId,
    string Kind,
    Guid? CategoryId,
    Stream Content,
    string FileName,
    string ContentType,
    long SizeInBytes) : ICommand<FileUploadResponse>, IMasterOwnedRequest;
