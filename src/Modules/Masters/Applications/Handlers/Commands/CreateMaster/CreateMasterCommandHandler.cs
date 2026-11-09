namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.CreateMaster;

public sealed class CreateMasterCommandHandler(
    IMasterService masterService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<CreateMasterCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        CreateMasterCommand request,
        CancellationToken cancellationToken)
    {
        var master = await masterService.CreateAsync(
            request.MasterId,
            request.FirstName,
            request.LastName,
            request.CategoryIds,
            request.ServiceArea,
            request.WorkSchedule,
            cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterProfileCreatedEvent(
                master.Id,
                master.FirstName,
                master.LastName,
                [.. master.Categories.Select(category => category.CategoryId)],
                MastersEvents.ChangedAt(master)),
            cancellationToken);

        return master;
    }
}
