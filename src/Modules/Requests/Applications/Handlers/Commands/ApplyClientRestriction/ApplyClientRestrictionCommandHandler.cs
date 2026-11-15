namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ApplyClientRestriction;

public sealed class ApplyClientRestrictionCommandHandler(
    IRestrictionService restrictionService) : ICommandHandler<ApplyClientRestrictionCommand>
{
    public async Task Handle(
        ApplyClientRestrictionCommand request,
        CancellationToken cancellationToken)
    {
        await restrictionService.ApplyAsync(
            request.RestrictionId,
            request.ClientId,
            request.Kind,
            request.ImposedAt,
            request.ExpiresAt,
            cancellationToken);
    }
}