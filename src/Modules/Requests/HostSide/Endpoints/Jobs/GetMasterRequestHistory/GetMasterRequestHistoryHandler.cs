using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequestHistory;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.GetMasterRequestHistory;

public static class GetMasterRequestHistoryHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetMasterRequestHistoryQuery(currentUserSession.GetRequiredUserId(), requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}