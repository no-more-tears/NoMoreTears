using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NoMoreTears.Modules.Identity.Contracts.Constants;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;
using NoMoreTears.Shared.HostSide.Endpoints.Extensions;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.Login;

public sealed class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Login, LoginHandler.Handle)
           .WithName(EndpointName.Login)
           .WithTags(EndpointTag.Identity)
           .AllowAnonymous()
           .WithRateLimit(maxRequests: 10, windowSeconds: 60)
           .WithThrottle(requestsPerSecond: 1, burstSeconds: 3.0);
    }
}
