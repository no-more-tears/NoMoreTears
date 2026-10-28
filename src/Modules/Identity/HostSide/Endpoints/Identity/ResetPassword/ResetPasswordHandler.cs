using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ResetPassword;
using NoMoreTears.Modules.Identity.Contracts.Requests;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.ResetPassword;

public static class ResetPasswordHandler
{
    public static async Task<IResult> Handle(
        [FromBody] ResetPasswordRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(
            request.UserId,
            request.Token,
            request.NewPassword);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}
