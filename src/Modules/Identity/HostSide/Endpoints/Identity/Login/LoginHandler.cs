using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Login;
using NoMoreTears.Modules.Identity.Contracts.Requests;
using NoMoreTears.Modules.Identity.HostSide.Endpoints.Extensions;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Utilities.Helpers;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.Login;

public static class LoginHandler
{
    public static async Task<IResult> Handle(
        [FromBody] LoginRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            request.Identifier,
            request.Password,
            currentUserSession.ToClientContext());

        var response = await sender.Send(command, cancellationToken);

        CookieHelper.SetAccessTokenCookie(response.Tokens.AccessToken, httpContext.Response.Cookies);
        CookieHelper.SetRefreshTokenCookie(response.Tokens.RefreshToken, httpContext.Response.Cookies);

        return Results.Ok(response);
    }
}