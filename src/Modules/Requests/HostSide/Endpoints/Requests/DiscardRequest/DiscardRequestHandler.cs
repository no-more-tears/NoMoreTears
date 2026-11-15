using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DiscardRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.DiscardRequest;

public static class DiscardRequestHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DiscardRequestCommand(currentUserSession.GetRequiredUserId(), requestId);

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}