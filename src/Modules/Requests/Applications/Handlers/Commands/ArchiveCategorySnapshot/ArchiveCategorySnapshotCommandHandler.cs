namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ArchiveCategorySnapshot;

public sealed class ArchiveCategorySnapshotCommandHandler(
    IMastersSnapshotService snapshotService) : ICommandHandler<ArchiveCategorySnapshotCommand>
{
    public async Task Handle(
        ArchiveCategorySnapshotCommand request,
        CancellationToken cancellationToken)
    {
        await snapshotService.ArchiveCategoryAsync(request.CategoryId, request.ArchivedAt, cancellationToken);
    }
}