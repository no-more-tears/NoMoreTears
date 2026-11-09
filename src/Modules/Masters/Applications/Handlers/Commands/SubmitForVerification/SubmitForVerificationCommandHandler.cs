using NoMoreTears.Modules.Masters.Domain.Enums;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.SubmitForVerification;

public sealed class SubmitForVerificationCommandHandler(
    IMasterService masterService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<SubmitForVerificationCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        SubmitForVerificationCommand request,
        CancellationToken cancellationToken)
    {
        var master = await masterService.SubmitForVerificationAsync(request.MasterId, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterSubmittedForVerificationEvent(
                master.Id,
                MastersEvents.PendingCategoryIds(master),
                master.ServiceAreaStatus == nameof(ReviewStatus.Pending),
                master.RevokedAt is not null,
                MastersEvents.ChangedAt(master)),
            cancellationToken);

        return master;
    }
}
