using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RefuseRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.RefuseRequest;

public static class RefuseRequestHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromBody] ReasonRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RefuseRequestCommand(currentUserSession.GetRequiredUserId(), requestId, request.Reason);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}