using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UpdateCategory;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.UpdateCategory;

public static class UpdateCategoryHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid categoryId,
        [FromBody] UpdateCategoryRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(categoryId, request.Name, request.Description);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
