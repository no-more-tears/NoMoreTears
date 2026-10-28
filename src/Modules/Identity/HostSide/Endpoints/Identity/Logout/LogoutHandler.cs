using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Logout;
using NoMoreTears.Modules.Identity.Contracts.Requests;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.Logout;

public static class LogoutHandler
{
    public static async Task<IResult> Handle(
        [FromBody] LogoutRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new LogoutCommand(
            request.RefreshToken);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}
