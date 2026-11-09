using NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetFileUpload;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.GetFileUpload;

public static class GetFileUploadHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid uploadId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetFileUploadQuery(currentUserSession.GetRequiredUserId(), uploadId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
