using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.SubmitForVerification;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.SubmitForVerification;

public static class SubmitForVerificationHandler
{
    public static async Task<IResult> Handle(
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new SubmitForVerificationCommand(currentUserSession.GetRequiredUserId());

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
