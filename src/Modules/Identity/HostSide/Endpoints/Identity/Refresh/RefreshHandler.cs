using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Refresh;
using NoMoreTears.Modules.Identity.Contracts.Requests;
using NoMoreTears.Modules.Identity.HostSide.Endpoints.Extensions;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.Refresh;

public static class RefreshHandler
{
    public static async Task<IResult> Handle(
        [FromBody] RefreshRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RefreshCommand(
            request.RefreshToken,
            currentUserSession.ToClientContext());

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
