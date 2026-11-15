using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequestHistory;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Support.GetRequestHistory;

public static class GetRequestHistoryHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetRequestHistoryQuery(requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}