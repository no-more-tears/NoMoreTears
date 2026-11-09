using NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCategories;
using NoMoreTears.Shared.Common.Exceptions.Client;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.GetCategories;

public static class GetCategoriesHandler
{
    public static async Task<IResult> Handle(
        [FromQuery] bool includeArchived,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        if (includeArchived && !currentUserSession.HasRole(UserRole.PlatformAdministrator))
        {
            throw new ForbiddenException("Only a platform administrator can see archived categories.");
        }

        var command = new GetCategoriesQuery(includeArchived);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
