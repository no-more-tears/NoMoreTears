namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.UploadMediaFile;

public sealed class UploadMediaFileCommandHandler(
    IMediaUploadService mediaUploadService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<UploadMediaFileCommand, MediaUploadResponse>
{
    public async Task<MediaUploadResponse> Handle(
        UploadMediaFileCommand request,
        CancellationToken cancellationToken)
    {
        var staged = await mediaUploadService.StageAsync(
            request.ClientId,
            request.RequestId,
            request.Content,
            request.FileName,
            request.ContentType,
            request.SizeInBytes,
            cancellationToken);

        await eventPublisher.PublishAsync(
            new RequestMediaStagedEvent(staged.Upload.Id, staged.Upload.RequestId, staged.ClientId, staged.Upload.CreatedAt!.Value),
            cancellationToken);

        return staged.Upload;
    }
}