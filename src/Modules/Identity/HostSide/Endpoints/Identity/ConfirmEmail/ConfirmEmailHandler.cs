using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ConfirmEmail;
using NoMoreTears.Modules.Identity.Contracts.Requests;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.ConfirmEmail;

public static class ConfirmEmailHandler
{
    public static async Task<IResult> Handle(
        [FromBody] ConfirmEmailRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmEmailCommand(
            request.UserId,
            request.Token);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}