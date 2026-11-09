using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UploadAvatar;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.UploadAvatar;

public static class UploadAvatarHandler
{
    public static async Task<IResult> Handle(
        IFormFile file,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        await using var content = file.OpenReadStream();

        var command = new UploadAvatarCommand(currentUserSession.GetRequiredUserId(), content, file.FileName, file.ContentType, file.Length);

        var response = await sender.Send(command, cancellationToken);

        return Results.AcceptedAtRoute(EndpointName.GetFileUpload, new { uploadId = response.Id }, response);
    }
}
