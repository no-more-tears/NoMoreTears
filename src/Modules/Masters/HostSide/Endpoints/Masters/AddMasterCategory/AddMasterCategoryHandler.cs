using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.AddMasterCategory;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.AddMasterCategory;

public static class AddMasterCategoryHandler
{
    public static async Task<IResult> Handle(
        [FromBody] AddCategoryRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AddMasterCategoryCommand(currentUserSession.GetRequiredUserId(), request.CategoryId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
