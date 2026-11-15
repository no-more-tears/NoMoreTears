using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.CreateRequest;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.CreateRequest;

public static class CreateRequestHandler
{
    public static async Task<IResult> Handle(
        [FromBody] CreateRequestRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateRequestCommand(currentUserSession.GetRequiredUserId(), request.CategoryId, request.Address, request.Description);

        var response = await sender.Send(command, cancellationToken);

        return Results.CreatedAtRoute(EndpointName.GetOwnRequest, new { requestId = response.Id }, response);
    }
}