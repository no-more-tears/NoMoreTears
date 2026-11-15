using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.PublishRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.PublishRequest;

public static class PublishRequestHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new PublishRequestCommand(currentUserSession.GetRequiredUserId(), requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}