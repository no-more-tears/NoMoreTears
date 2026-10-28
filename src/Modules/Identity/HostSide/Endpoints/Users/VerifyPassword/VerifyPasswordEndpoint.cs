using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NoMoreTears.Modules.Identity.Contracts.Constants;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;
using NoMoreTears.Shared.HostSide.Endpoints.Extensions;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.VerifyPassword;

public sealed class VerifyPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.VerifyPassword, VerifyPasswordHandler.Handle)
            .WithName(EndpointName.VerifyPassword)
            .WithTags(EndpointTag.Users)
            .RequireAuthorization()
            .WithRateLimit(maxRequests: 5, windowSeconds: 60);
    }
}
