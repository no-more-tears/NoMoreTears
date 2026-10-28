using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.LogoutAll;
using NoMoreTears.Modules.Identity.HostSide.Endpoints.Extensions;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.LogoutAll;

public static class LogoutAllHandler
{
    public static async Task<IResult> Handle(
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new LogoutAllCommand(
            currentUserSession.GetRequiredUserId());

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}
