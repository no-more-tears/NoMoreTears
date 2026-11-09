using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectMasterCategory;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.RejectMasterCategory;

public static class RejectMasterCategoryHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid masterId,
        [FromRoute] Guid categoryId,
        [FromBody] RejectionRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RejectMasterCategoryCommand(currentUserSession.GetRequiredUserId(), masterId, categoryId, request.Reason);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
