using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnMediaFile;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.GetOwnMediaFile;

public static class GetOwnMediaFileHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromRoute] Guid mediaFileId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetOwnMediaFileQuery(currentUserSession.GetRequiredUserId(), requestId, mediaFileId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Stream(response.Content, response.ContentType);
    }
}