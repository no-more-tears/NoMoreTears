using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.UpdateMfa;
using NoMoreTears.Modules.Identity.Contracts.Requests;
using NoMoreTears.Modules.Identity.HostSide.Endpoints.Extensions;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.UpdateMfa;

public static class UpdateMfaHandler
{
    public static async Task<IResult> Handle(
        [FromBody] UpdateMfaRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMfaCommand(
            currentUserSession.GetRequiredUserId(),
            request.Password,
            request.Enabled);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}