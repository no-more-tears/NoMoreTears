namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.LiftClientRestriction;

public sealed class LiftClientRestrictionCommandHandler(
    IRestrictionService restrictionService) : ICommandHandler<LiftClientRestrictionCommand>
{
    public async Task Handle(
        LiftClientRestrictionCommand request,
        CancellationToken cancellationToken)
    {
        await restrictionService.LiftAsync(request.RestrictionId, request.LiftedAt, cancellationToken);
    }
}