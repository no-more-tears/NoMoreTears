using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequests;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.GetOwnRequests;

public static class GetOwnRequestsHandler
{
    public static async Task<IResult> Handle(
        [FromQuery] string? status,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetOwnRequestsQuery(currentUserSession.GetRequiredUserId(), status, pageNumber ?? 1, pageSize ?? 20);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}