using NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetOwnDocument;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.GetOwnDocument;

public static class GetOwnDocumentHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid documentId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetOwnDocumentQuery(currentUserSession.GetRequiredUserId(), documentId);

        var response = await sender.Send(command, cancellationToken);

        return Results.File(response.Content, response.ContentType, response.FileName);
    }
}
