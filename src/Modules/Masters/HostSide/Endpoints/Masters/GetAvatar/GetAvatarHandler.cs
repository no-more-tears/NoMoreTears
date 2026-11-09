using NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetAvatar;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.GetAvatar;

public static class GetAvatarHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid masterId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetAvatarQuery(masterId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Stream(response.Content, response.ContentType);
    }
}
