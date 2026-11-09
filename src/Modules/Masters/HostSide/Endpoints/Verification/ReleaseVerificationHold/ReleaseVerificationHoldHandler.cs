using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ReleaseVerificationHold;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.ReleaseVerificationHold;

public static class ReleaseVerificationHoldHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid masterId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ReleaseVerificationHoldCommand(masterId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
