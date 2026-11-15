namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ProcessMediaUpload;

public sealed class ProcessMediaUploadCommandHandler(
    IMediaUploadService mediaUploadService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<ProcessMediaUploadCommand>
{
    public async Task Handle(
        ProcessMediaUploadCommand request,
        CancellationToken cancellationToken)
    {
        var result = await mediaUploadService.ProcessAsync(request.UploadId, cancellationToken);

        if (result is null)
        {
            return;
        }

        var upload = result.Upload;
        var processedAt = upload.ProcessedAt!.Value;

        if (result.Request is null)
        {
            await eventPublisher.PublishAsync(
                new RequestMediaUploadFailedEvent(result.ClientId, upload.RequestId, upload.Id, upload.FileName, upload.FailureReason ?? string.Empty, processedAt),
                cancellationToken);
        }
        else
        {
            await eventPublisher.PublishAsync(
                new RequestMediaUploadCompletedEvent(result.ClientId, upload.RequestId, upload.Id, upload.MediaFileId!.Value, upload.FileName, processedAt),
                cancellationToken);
            await eventPublisher.PublishAsync(RequestsEvents.DraftUpdated(result.Request), cancellationToken);
        }

        await eventPublisher.PublishObsoleteFilesAsync(upload.RequestId, result.ObsoleteFiles, processedAt, cancellationToken);
    }
}