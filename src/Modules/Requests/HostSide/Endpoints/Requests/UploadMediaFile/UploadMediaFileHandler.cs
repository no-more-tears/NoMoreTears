using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.UploadMediaFile;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.UploadMediaFile;

public static class UploadMediaFileHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        IFormFile file,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        await using var content = file.OpenReadStream();

        var command = new UploadMediaFileCommand(currentUserSession.GetRequiredUserId(), requestId, content, file.FileName, file.ContentType, file.Length);

        var response = await sender.Send(command, cancellationToken);

        return Results.AcceptedAtRoute(EndpointName.GetMediaUpload, new { requestId = response.RequestId, uploadId = response.Id }, response);
    }
}