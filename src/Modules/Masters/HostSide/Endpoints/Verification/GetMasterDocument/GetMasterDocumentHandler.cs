using NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMasterDocument;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.GetMasterDocument;

public static class GetMasterDocumentHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid masterId,
        [FromRoute] Guid documentId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetMasterDocumentQuery(masterId, documentId);

        var response = await sender.Send(command, cancellationToken);

        return Results.File(response.Content, response.ContentType, response.FileName);
    }
}
