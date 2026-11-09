using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveAvatar;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.RemoveAvatar;

public static class RemoveAvatarHandler
{
    public static async Task<IResult> Handle(
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RemoveAvatarCommand(currentUserSession.GetRequiredUserId());

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
