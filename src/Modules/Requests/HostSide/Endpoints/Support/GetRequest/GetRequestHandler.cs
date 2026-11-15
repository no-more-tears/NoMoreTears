using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Support.GetRequest;

public static class GetRequestHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetRequestQuery(requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}