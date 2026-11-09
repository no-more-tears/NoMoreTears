using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeFullName;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.ChangeFullName;

public static class ChangeFullNameHandler
{
    public static async Task<IResult> Handle(
        [FromBody] ChangeFullNameRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ChangeFullNameCommand(currentUserSession.GetRequiredUserId(), request.FirstName, request.LastName);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
