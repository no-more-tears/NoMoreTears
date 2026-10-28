using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Queries.GetCurrentUser;
using NoMoreTears.Modules.Identity.HostSide.Endpoints.Extensions;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.GetCurrentUser;

public static class GetCurrentUserHandler
{
    public static async Task<IResult> Handle(
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetCurrentUserQuery(
            currentUserSession.GetRequiredUserId());

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
