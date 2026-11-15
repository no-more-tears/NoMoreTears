using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.AcceptRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.AcceptRequest;

public static class AcceptRequestHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromBody] AcceptRequestRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AcceptRequestCommand(currentUserSession.GetRequiredUserId(), requestId, request.ArrivalIntervalInHours);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}