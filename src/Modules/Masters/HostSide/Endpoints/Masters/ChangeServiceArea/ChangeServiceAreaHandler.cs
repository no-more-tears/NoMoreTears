using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeServiceArea;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.ChangeServiceArea;

public static class ChangeServiceAreaHandler
{
    public static async Task<IResult> Handle(
        [FromBody] ServiceAreaRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ChangeServiceAreaCommand(currentUserSession.GetRequiredUserId(), request);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
