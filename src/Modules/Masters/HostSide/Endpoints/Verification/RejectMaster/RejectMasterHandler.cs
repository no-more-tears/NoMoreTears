using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectMaster;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.RejectMaster;

public static class RejectMasterHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid masterId,
        [FromBody] RejectionRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RejectMasterCommand(currentUserSession.GetRequiredUserId(), masterId, request.Reason);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
