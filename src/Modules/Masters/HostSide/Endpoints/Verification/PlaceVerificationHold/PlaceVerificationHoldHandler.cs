using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.PlaceVerificationHold;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.PlaceVerificationHold;

public static class PlaceVerificationHoldHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid masterId,
        [FromBody] PlaceVerificationHoldRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new PlaceVerificationHoldCommand(masterId, request.Reason);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
