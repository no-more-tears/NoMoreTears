using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveMasterCategory;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.RemoveMasterCategory;

public static class RemoveMasterCategoryHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid categoryId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RemoveMasterCategoryCommand(currentUserSession.GetRequiredUserId(), categoryId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
