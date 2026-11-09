using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeWorkSchedule;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.ChangeWorkSchedule;

public static class ChangeWorkScheduleHandler
{
    public static async Task<IResult> Handle(
        [FromBody] WorkScheduleRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ChangeWorkScheduleCommand(currentUserSession.GetRequiredUserId(), request);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
