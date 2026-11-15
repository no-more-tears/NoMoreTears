using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ReportNoShow;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ReportNoShow;

public static class ReportNoShowHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid requestId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ReportNoShowCommand(currentUserSession.GetRequiredUserId(), requestId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}