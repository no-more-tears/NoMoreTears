namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UploadAvatar;

public sealed class UploadAvatarCommandHandler(
    IFileUploadService fileUploadService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<UploadAvatarCommand, FileUploadResponse>
{
    public async Task<FileUploadResponse> Handle(
        UploadAvatarCommand request,
        CancellationToken cancellationToken)
    {
        var staged = await fileUploadService.StageAvatarAsync(
            request.MasterId,
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
