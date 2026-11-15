using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.MarkRequestAsDone;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.MarkRequestAsDone;

public static class MarkRequestAsDoneHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new MarkRequestAsDoneCommand(currentUserSession.GetRequiredUserId(), requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}