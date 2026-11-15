using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ObjectToCompletion;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ObjectToCompletion;

public static class ObjectToCompletionHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromBody] ReasonRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ObjectToCompletionCommand(currentUserSession.GetRequiredUserId(), requestId, request.Reason);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}