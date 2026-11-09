using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UploadDocument;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.UploadDocument;

public static class UploadDocumentHandler
{
    public static async Task<IResult> Handle(
        IFormFile file,
        [FromForm] string kind,
        [FromForm] Guid? categoryId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        await using var content = file.OpenReadStream();

        var command = new UploadDocumentCommand(currentUserSession.GetRequiredUserId(), kind, categoryId, content, file.FileName, file.ContentType, file.Length);

        var response = await sender.Send(command, cancellationToken);

        return Results.AcceptedAtRoute(EndpointName.GetFileUpload, new { uploadId = response.Id }, response);
    }
}
