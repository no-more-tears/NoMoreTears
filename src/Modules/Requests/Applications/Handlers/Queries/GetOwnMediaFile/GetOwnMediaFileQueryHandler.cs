namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnMediaFile;

public sealed class GetOwnMediaFileQueryHandler(
    IMediaUploadService mediaUploadService) : IQueryHandler<GetOwnMediaFileQuery, FileContentResponse>
{
    public async Task<FileContentResponse> Handle(
        GetOwnMediaFileQuery request,
        CancellationToken cancellationToken)
    {
        return await mediaUploadService.OpenForClientAsync(request.ClientId, request.RequestId, request.MediaFileId, cancellationToken);
    }
}