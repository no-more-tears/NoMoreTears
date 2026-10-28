using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NoMoreTears.Modules.Identity.Contracts.Constants;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;
using NoMoreTears.Shared.HostSide.Endpoints.Extensions;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.ForgotPassword;

public sealed class ForgotPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.ForgotPassword, ForgotPasswordHandler.Handle)
            .WithName(EndpointName.ForgotPassword)
            .WithTags(EndpointTag.Identity)
            .AllowAnonymous()
            .WithRateLimit(maxRequests: 3, windowSeconds: 60);
    }
}
