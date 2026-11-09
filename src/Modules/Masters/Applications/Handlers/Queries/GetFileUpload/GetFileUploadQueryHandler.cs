namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetFileUpload;

public sealed class GetFileUploadQueryHandler(
    IFileUploadService fileUploadService) : IQueryHandler<GetFileUploadQuery, FileUploadResponse>
{
    public async Task<FileUploadResponse> Handle(
        GetFileUploadQuery request,
        CancellationToken cancellationToken)
    {
        return await fileUploadService.GetAsync(request.MasterId, request.UploadId, cancellationToken);
    }
}
