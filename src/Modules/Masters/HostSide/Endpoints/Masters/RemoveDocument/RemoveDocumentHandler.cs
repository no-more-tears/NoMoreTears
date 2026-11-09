using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveDocument;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.RemoveDocument;

public static class RemoveDocumentHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid documentId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RemoveDocumentCommand(currentUserSession.GetRequiredUserId(), documentId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
