using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.CreateMaster;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.CreateMaster;

public static class CreateMasterHandler
{
    public static async Task<IResult> Handle(
        [FromBody] CreateMasterRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateMasterCommand(
            currentUserSession.GetRequiredUserId(),
            request.FirstName,
            request.LastName,
            request.CategoryIds,
            request.ServiceArea,
            request.WorkSchedule);

        var response = await sender.Send(command, cancellationToken);

        return Results.CreatedAtRoute(EndpointName.GetCurrentMaster, null, response);
    }
}
