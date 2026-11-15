using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.GetMasterRequest;

public static class GetMasterRequestHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetMasterRequestQuery(currentUserSession.GetRequiredUserId(), requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}