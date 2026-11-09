using NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCategory;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.GetCategory;

public static class GetCategoryHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid categoryId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetCategoryQuery(categoryId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
