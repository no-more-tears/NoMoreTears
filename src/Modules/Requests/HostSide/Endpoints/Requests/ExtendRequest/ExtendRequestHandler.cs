using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ExtendRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ExtendRequest;

public static class ExtendRequestHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ExtendRequestCommand(currentUserSession.GetRequiredUserId(), requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}