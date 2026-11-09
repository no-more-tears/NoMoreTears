using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeAvailability;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.ChangeAvailability;

public static class ChangeAvailabilityHandler
{
    public static async Task<IResult> Handle(
        [FromBody] ChangeAvailabilityRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ChangeAvailabilityCommand(currentUserSession.GetRequiredUserId(), request.IsAvailable);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
