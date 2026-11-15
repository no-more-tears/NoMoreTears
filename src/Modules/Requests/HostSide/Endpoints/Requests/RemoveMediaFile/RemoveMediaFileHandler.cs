using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RemoveMediaFile;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.RemoveMediaFile;

public static class RemoveMediaFileHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromRoute] Guid mediaFileId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RemoveMediaFileCommand(currentUserSession.GetRequiredUserId(), requestId, mediaFileId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}