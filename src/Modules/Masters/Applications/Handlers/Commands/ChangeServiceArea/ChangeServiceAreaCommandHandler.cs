using NoMoreTears.Modules.Masters.Contracts.Requests;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeServiceArea;

public sealed class ChangeServiceAreaCommandHandler(
    IMasterService masterService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<ChangeServiceAreaCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        ChangeServiceAreaCommand request,
        CancellationToken cancellationToken)
    {
        var master = await masterService.ChangeServiceAreaAsync(request.MasterId, request.ServiceArea, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterServiceAreaChangedEvent(master.Id, master.ServiceArea, master.ServiceAreaStatus, MastersEvents.ChangedAt(master)),
            cancellationToken);
        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);

        return master;
    }
}
