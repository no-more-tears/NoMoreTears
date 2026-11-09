namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ProcessFileUpload;

public sealed class ProcessFileUploadCommandHandler(
    IFileUploadService fileUploadService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<ProcessFileUploadCommand>
{
    public async Task Handle(
        ProcessFileUploadCommand request,
        CancellationToken cancellationToken)
    {
        var result = await fileUploadService.ProcessAsync(request.UploadId, cancellationToken);

        if (result is null)
        {
            return;
        }

        var upload = result.Upload;
        var processedAt = upload.ProcessedAt!.Value;

        if (result.Master is null)
        {
            await eventPublisher.PublishAsync(
                new MasterFileUploadFailedEvent(result.MasterId, upload.Id, upload.Purpose, upload.FileName, upload.FailureReason ?? string.Empty, processedAt),
                cancellationToken);
        }
        else
        {
            await eventPublisher.PublishAsync(
                new MasterFileUploadCompletedEvent(result.MasterId, upload.Id, upload.Purpose, upload.CategoryId, upload.DocumentId, upload.FileName, processedAt),
                cancellationToken);

            if (upload.DocumentId is null)
            {
                await eventPublisher.PublishAsync(new MasterAvatarChangedEvent(result.MasterId, true, processedAt), cancellationToken);
            }
        }

        await eventPublisher.PublishAsync(new MasterFilesObsoletedEvent(result.MasterId, result.ObsoleteFiles, processedAt), cancellationToken);
    }
}
