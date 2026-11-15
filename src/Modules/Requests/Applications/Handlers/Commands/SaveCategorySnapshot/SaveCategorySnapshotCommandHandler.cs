namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SaveCategorySnapshot;

public sealed class SaveCategorySnapshotCommandHandler(
    IMastersSnapshotService snapshotService) : ICommandHandler<SaveCategorySnapshotCommand>
{
    public async Task Handle(
        SaveCategorySnapshotCommand request,
        CancellationToken cancellationToken)
    {
        await snapshotService.SaveCategoryAsync(request.CategoryId, request.Name, request.ChangedAt, cancellationToken);
    }
}