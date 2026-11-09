using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyMaster;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.VerifyMaster;

public static class VerifyMasterHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid masterId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new VerifyMasterCommand(currentUserSession.GetRequiredUserId(), masterId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
