using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMediaUpload;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.GetMediaUpload;

public static class GetMediaUploadHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromRoute] Guid uploadId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetMediaUploadQuery(currentUserSession.GetRequiredUserId(), requestId, uploadId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}