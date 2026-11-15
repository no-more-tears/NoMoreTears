namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DeleteObsoleteFiles;

public sealed class DeleteObsoleteFilesCommandHandler(
    IMediaUploadService mediaUploadService) : ICommandHandler<DeleteObsoleteFilesCommand>
{
    public async Task Handle(
        DeleteObsoleteFilesCommand request,
        CancellationToken cancellationToken)
    {
        await mediaUploadService.DeleteFilesAsync(request.Files, cancellationToken);
    }
}