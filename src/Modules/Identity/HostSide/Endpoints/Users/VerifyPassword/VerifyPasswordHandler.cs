using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.VerifyPassword;
using NoMoreTears.Modules.Identity.Contracts.Requests;
using NoMoreTears.Modules.Identity.HostSide.Endpoints.Extensions;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.VerifyPassword;

public static class VerifyPasswordHandler
{
    public static async Task<IResult> Handle(
        [FromBody] VerifyPasswordRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new VerifyPasswordCommand(
            currentUserSession.GetRequiredUserId(),
            request.Password);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}
