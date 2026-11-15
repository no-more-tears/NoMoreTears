using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.GetOwnRequest;

public static class GetOwnRequestHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetOwnRequestQuery(currentUserSession.GetRequiredUserId(), requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}