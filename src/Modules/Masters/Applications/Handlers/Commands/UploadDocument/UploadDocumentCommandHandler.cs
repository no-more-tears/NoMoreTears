namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UploadDocument;

public sealed class UploadDocumentCommandHandler(
    IFileUploadService fileUploadService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<UploadDocumentCommand, FileUploadResponse>
{
    public async Task<FileUploadResponse> Handle(
        UploadDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var staged = await fileUploadService.StageDocumentAsync(
            request.MasterId,
            request.Kind,
            request.CategoryId,
            request.Content,
            request.FileName,
            request.ContentType,
            request.SizeInBytes,
            cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterFileStagedEvent(staged.Upload.Id, staged.MasterId, staged.Upload.Purpose, staged.Upload.CreatedAt!.Value),
            cancellationToken);

        return staged.Upload;
    }
}
