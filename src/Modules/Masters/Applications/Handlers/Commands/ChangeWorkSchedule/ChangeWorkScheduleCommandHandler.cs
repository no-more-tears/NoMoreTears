using NoMoreTears.Modules.Masters.Contracts.Requests;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeWorkSchedule;

public sealed class ChangeWorkScheduleCommandHandler(
    IMasterService masterService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<ChangeWorkScheduleCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        ChangeWorkScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var master = await masterService.ChangeWorkScheduleAsync(request.MasterId, request.WorkSchedule, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterWorkScheduleChangedEvent(master.Id, master.WorkSchedule, MastersEvents.ChangedAt(master)),
            cancellationToken);
        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);

        return master;
    }
}
