using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetOwnDocument;

public sealed record GetOwnDocumentQuery(
    Guid MasterId,
    Guid DocumentId) : IQuery<FileContentResponse>, IMasterOwnedRequest;
