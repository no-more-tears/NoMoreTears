using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestCategory;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ChangeRequestCategory;

public static class ChangeRequestCategoryHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromBody] ChangeCategoryRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ChangeRequestCategoryCommand(currentUserSession.GetRequiredUserId(), requestId, request.CategoryId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}