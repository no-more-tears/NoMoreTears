using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestAddress;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ChangeRequestAddress;

public static class ChangeRequestAddressHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromBody] AddressRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ChangeRequestAddressCommand(currentUserSession.GetRequiredUserId(), requestId, request);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}