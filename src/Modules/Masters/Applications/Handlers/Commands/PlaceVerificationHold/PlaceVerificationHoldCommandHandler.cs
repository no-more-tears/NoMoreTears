namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.PlaceVerificationHold;

public sealed class PlaceVerificationHoldCommandHandler(
    IVerificationService verificationService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<PlaceVerificationHoldCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        PlaceVerificationHoldCommand request,
        CancellationToken cancellationToken)
    {
        var master = await verificationService.PlaceHoldAsync(request.MasterId, request.Reason, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterVerificationHoldPlacedEvent(master.Id, master.VerificationHold!.Reason, MastersEvents.ChangedAt(master)),
            cancellationToken);

        return master;
    }
}
