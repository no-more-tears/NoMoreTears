using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ConfirmCompletion;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ConfirmCompletion;

public static class ConfirmCompletionHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmCompletionCommand(currentUserSession.GetRequiredUserId(), requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}