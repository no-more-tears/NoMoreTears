namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMasterDocument;

public sealed class GetMasterDocumentQueryHandler(
    IFileUploadService fileUploadService) : IQueryHandler<GetMasterDocumentQuery, FileContentResponse>
{
    public async Task<FileContentResponse> Handle(
        GetMasterDocumentQuery request,
        CancellationToken cancellationToken)
    {
        return await fileUploadService.OpenDocumentAsync(request.MasterId, request.DocumentId, cancellationToken);
    }
}
