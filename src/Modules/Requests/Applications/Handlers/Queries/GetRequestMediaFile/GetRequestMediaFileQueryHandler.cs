namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequestMediaFile;

public sealed class GetRequestMediaFileQueryHandler(
    IMediaUploadService mediaUploadService) : IQueryHandler<GetRequestMediaFileQuery, FileContentResponse>
{
    public async Task<FileContentResponse> Handle(
        GetRequestMediaFileQuery request,
        CancellationToken cancellationToken)
    {
        return await mediaUploadService.OpenForSupportAsync(request.RequestId, request.MediaFileId, cancellationToken);
    }
}