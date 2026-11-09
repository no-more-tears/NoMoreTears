namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.DeleteObsoleteFiles;

public sealed class DeleteObsoleteFilesCommandHandler(
    IFileUploadService fileUploadService) : ICommandHandler<DeleteObsoleteFilesCommand>
{
    public async Task Handle(
        DeleteObsoleteFilesCommand request,
        CancellationToken cancellationToken)
    {
        await fileUploadService.DeleteFilesAsync(request.Files, cancellationToken);
    }
}
