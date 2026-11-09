using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyServiceArea;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.VerifyServiceArea;

public static class VerifyServiceAreaHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid masterId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new VerifyServiceAreaCommand(currentUserSession.GetRequiredUserId(), masterId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
