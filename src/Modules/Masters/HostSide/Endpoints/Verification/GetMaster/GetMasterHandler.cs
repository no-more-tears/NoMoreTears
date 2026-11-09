using NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMaster;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.GetMaster;

public static class GetMasterHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid masterId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetMasterQuery(masterId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
