using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RepublishRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.RepublishRequest;

public static class RepublishRequestHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RepublishRequestCommand(currentUserSession.GetRequiredUserId(), requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}