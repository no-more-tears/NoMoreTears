using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NoMoreTears.Modules.Identity.Contracts.Constants;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;
using NoMoreTears.Shared.HostSide.Endpoints.Extensions;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.Refresh;

public sealed class RefreshEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Refresh, RefreshHandler.Handle)
            .WithName(EndpointName.Refresh)
            .WithTags(EndpointTag.Identity)
            .AllowAnonymous()
            .WithRateLimit(maxRequests: 30, windowSeconds: 60);
    }
}
