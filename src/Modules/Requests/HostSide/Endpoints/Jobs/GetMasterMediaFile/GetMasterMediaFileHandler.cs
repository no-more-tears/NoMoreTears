using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterMediaFile;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.GetMasterMediaFile;

public static class GetMasterMediaFileHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromRoute] Guid mediaFileId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetMasterMediaFileQuery(currentUserSession.GetRequiredUserId(), requestId, mediaFileId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Stream(response.Content, response.ContentType);
    }
}