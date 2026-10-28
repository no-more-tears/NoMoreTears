using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.RevokeSession;
using NoMoreTears.Modules.Identity.HostSide.Endpoints.Extensions;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.RevokeSession;

public static class RevokeSessionHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid sessionId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RevokeSessionCommand(
            currentUserSession.GetRequiredUserId(),
            sessionId);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}
