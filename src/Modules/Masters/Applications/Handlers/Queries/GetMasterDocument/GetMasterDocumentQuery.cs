namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMasterDocument;

public sealed record GetMasterDocumentQuery(
    Guid MasterId,
    Guid DocumentId) : IQuery<FileContentResponse>;
