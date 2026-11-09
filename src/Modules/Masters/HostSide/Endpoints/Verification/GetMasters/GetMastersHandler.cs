using NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMasters;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.GetMasters;

public static class GetMastersHandler
{
    public static async Task<IResult> Handle(
        [FromQuery] string? verification,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetMastersQuery(verification, pageNumber ?? 1, pageSize ?? 20);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
