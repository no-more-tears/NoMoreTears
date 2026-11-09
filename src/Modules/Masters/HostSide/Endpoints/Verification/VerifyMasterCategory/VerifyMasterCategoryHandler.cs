using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyMasterCategory;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.VerifyMasterCategory;

public static class VerifyMasterCategoryHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid masterId,
        [FromRoute] Guid categoryId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new VerifyMasterCategoryCommand(currentUserSession.GetRequiredUserId(), masterId, categoryId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
