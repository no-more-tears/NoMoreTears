using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ForgotPassword;
using NoMoreTears.Modules.Identity.Contracts.Requests;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.ForgotPassword;

public static class ForgotPasswordHandler
{
    public static async Task<IResult> Handle(
        [FromBody] ForgotPasswordRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ForgotPasswordCommand(
            request.Identifier);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}
