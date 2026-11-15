using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.CancelRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.CancelRequest;

public static class CancelRequestHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromBody] CancelRequestRequest? request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CancelRequestCommand(currentUserSession.GetRequiredUserId(), requestId, request?.Reason);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}