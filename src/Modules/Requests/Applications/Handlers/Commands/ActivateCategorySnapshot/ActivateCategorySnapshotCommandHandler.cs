namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ActivateCategorySnapshot;

public sealed class ActivateCategorySnapshotCommandHandler(
    IMastersSnapshotService snapshotService) : ICommandHandler<ActivateCategorySnapshotCommand>
{
    public async Task Handle(
        ActivateCategorySnapshotCommand request,
        CancellationToken cancellationToken)
    {
        await snapshotService.ActivateCategoryAsync(request.CategoryId, request.ActivatedAt, cancellationToken);
    }
}