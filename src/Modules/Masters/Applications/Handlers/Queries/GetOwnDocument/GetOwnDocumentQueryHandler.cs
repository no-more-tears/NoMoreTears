namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetOwnDocument;

public sealed class GetOwnDocumentQueryHandler(
    IFileUploadService fileUploadService) : IQueryHandler<GetOwnDocumentQuery, FileContentResponse>
{
    public async Task<FileContentResponse> Handle(
        GetOwnDocumentQuery request,
        CancellationToken cancellationToken)
    {
        return await fileUploadService.OpenDocumentAsync(request.MasterId, request.DocumentId, cancellationToken);
    }
}
