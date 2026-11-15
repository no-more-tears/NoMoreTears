namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SaveMasterAccess;

public sealed class SaveMasterAccessCommandHandler(
    IMastersSnapshotService snapshotService) : ICommandHandler<SaveMasterAccessCommand>
{
    public async Task Handle(
        SaveMasterAccessCommand request,
        CancellationToken cancellationToken)
    {
        await snapshotService.SaveMasterAccessAsync(
            request.MasterId,
            request.AcceptableCategoryIds,
            request.SuspendedUntil,
            request.ChangedAt,
            cancellationToken);
    }
}