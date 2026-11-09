using NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCurrentMaster;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.GetCurrentMaster;

public static class GetCurrentMasterHandler
{
    public static async Task<IResult> Handle(
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetCurrentMasterQuery(currentUserSession.GetRequiredUserId());

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
