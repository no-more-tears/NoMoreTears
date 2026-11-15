using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequests;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Support.GetRequests;

public static class GetRequestsHandler
{
    public static async Task<IResult> Handle(
        [FromQuery] string? status,
        [FromQuery] Guid? clientId,
        [FromQuery] Guid? masterId,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetRequestsQuery(status, clientId, masterId, pageNumber ?? 1, pageSize ?? 20);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}