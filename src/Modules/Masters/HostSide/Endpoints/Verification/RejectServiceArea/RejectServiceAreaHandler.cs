using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectServiceArea;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.RejectServiceArea;

public static class RejectServiceAreaHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid masterId,
        [FromBody] RejectionRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RejectServiceAreaCommand(currentUserSession.GetRequiredUserId(), masterId, request.Reason);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
