namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeFullName;

public sealed class ChangeFullNameCommandHandler(
    IMasterService masterService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<ChangeFullNameCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        ChangeFullNameCommand request,
        CancellationToken cancellationToken)
    {
        var master = await masterService.ChangeFullNameAsync(
            request.MasterId,
            request.FirstName,
            request.LastName,
            cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterFullNameChangedEvent(master.Id, master.FirstName, master.LastName, MastersEvents.ChangedAt(master)),
            cancellationToken);

        return master;
    }
}
