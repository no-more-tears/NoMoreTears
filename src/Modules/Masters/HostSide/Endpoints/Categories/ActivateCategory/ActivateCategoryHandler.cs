using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ActivateCategory;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.ActivateCategory;

public static class ActivateCategoryHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid categoryId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ActivateCategoryCommand(categoryId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
