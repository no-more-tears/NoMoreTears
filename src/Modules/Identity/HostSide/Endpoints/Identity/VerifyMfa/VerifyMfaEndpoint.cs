using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NoMoreTears.Modules.Identity.Contracts.Constants;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;
using NoMoreTears.Shared.HostSide.Endpoints.Extensions;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.VerifyMfa;

public sealed class VerifyMfaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.VerifyMfa, VerifyMfaHandler.Handle)
            .WithName(EndpointName.VerifyMfa)
            .WithTags(EndpointTag.Identity)
            .AllowAnonymous()
            .WithRateLimit(maxRequests: 5, windowSeconds: 60);
    }
}
