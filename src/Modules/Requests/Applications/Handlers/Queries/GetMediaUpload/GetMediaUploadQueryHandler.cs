namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMediaUpload;

public sealed class GetMediaUploadQueryHandler(
    IMediaUploadService mediaUploadService) : IQueryHandler<GetMediaUploadQuery, MediaUploadResponse>
{
    public async Task<MediaUploadResponse> Handle(
        GetMediaUploadQuery request,
        CancellationToken cancellationToken)
    {
        return await mediaUploadService.GetAsync(request.ClientId, request.RequestId, request.UploadId, cancellationToken);
    }
}