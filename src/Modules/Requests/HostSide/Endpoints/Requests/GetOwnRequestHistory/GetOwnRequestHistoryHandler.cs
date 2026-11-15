using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequestHistory;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.GetOwnRequestHistory;

public static class GetOwnRequestHistoryHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetOwnRequestHistoryQuery(currentUserSession.GetRequiredUserId(), requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}