using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ChangePassword;
using NoMoreTears.Modules.Identity.Contracts.Requests;
using NoMoreTears.Modules.Identity.HostSide.Endpoints.Extensions;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.ChangePassword;

public static class ChangePasswordHandler
{
    public static async Task<IResult> Handle(
        [FromBody] ChangePasswordRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand(
            currentUserSession.GetRequiredUserId(),
            request.CurrentPassword,
            request.NewPassword,
            currentUserSession.SessionId);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}
