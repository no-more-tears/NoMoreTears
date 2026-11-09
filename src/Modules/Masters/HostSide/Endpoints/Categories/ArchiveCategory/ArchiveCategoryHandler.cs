using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ArchiveCategory;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.ArchiveCategory;

public static class ArchiveCategoryHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid categoryId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ArchiveCategoryCommand(categoryId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
