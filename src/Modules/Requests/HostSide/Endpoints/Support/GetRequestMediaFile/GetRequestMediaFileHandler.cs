using NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequestMediaFile;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Support.GetRequestMediaFile;

public static class GetRequestMediaFileHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromRoute] Guid mediaFileId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetRequestMediaFileQuery(requestId, mediaFileId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Stream(response.Content, response.ContentType);
    }
}