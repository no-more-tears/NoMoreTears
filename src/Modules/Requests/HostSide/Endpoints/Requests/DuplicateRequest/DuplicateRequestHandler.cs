using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DuplicateRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.DuplicateRequest;

public static class DuplicateRequestHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromBody] DuplicateRequestRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DuplicateRequestCommand(currentUserSession.GetRequiredUserId(), requestId, request.Description);

        var response = await sender.Send(command, cancellationToken);

        return Results.CreatedAtRoute(EndpointName.GetOwnRequest, new { requestId = response.Id }, response);
    }
}