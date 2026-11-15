namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterMediaFile;

public sealed class GetMasterMediaFileQueryHandler(
    IMediaUploadService mediaUploadService) : IQueryHandler<GetMasterMediaFileQuery, FileContentResponse>
{
    public async Task<FileContentResponse> Handle(
        GetMasterMediaFileQuery request,
        CancellationToken cancellationToken)
    {
        return await mediaUploadService.OpenForMasterAsync(request.MasterId, request.RequestId, request.MediaFileId, cancellationToken);
    }
}