using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestDescription;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ChangeRequestDescription;

public static class ChangeRequestDescriptionHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromBody] ChangeDescriptionRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ChangeRequestDescriptionCommand(currentUserSession.GetRequiredUserId(), requestId, request.Description);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}