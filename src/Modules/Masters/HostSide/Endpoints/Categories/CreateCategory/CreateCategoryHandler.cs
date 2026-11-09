using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.CreateCategory;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.CreateCategory;

public static class CreateCategoryHandler
{
    public static async Task<IResult> Handle(
        [FromBody] CreateCategoryRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(request.Name, request.Description);

        var response = await sender.Send(command, cancellationToken);

        return Results.CreatedAtRoute(EndpointName.GetCategory, new { categoryId = response.Id }, response);
    }
}
