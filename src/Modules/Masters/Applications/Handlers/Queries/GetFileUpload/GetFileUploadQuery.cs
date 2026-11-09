using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetFileUpload;

public sealed record GetFileUploadQuery(
    Guid MasterId,
    Guid UploadId) : IQuery<FileUploadResponse>, IMasterOwnedRequest;
