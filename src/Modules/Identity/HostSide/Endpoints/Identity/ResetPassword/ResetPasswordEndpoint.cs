using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NoMoreTears.Modules.Identity.Contracts.Constants;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;
using NoMoreTears.Shared.HostSide.Endpoints.Extensions;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.ResetPassword;

public sealed class ResetPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.ResetPassword, ResetPasswordHandler.Handle)
            .WithName(EndpointName.ResetPassword)
            .WithTags(EndpointTag.Identity)
            .AllowAnonymous()
            .WithRateLimit(maxRequests: 5, windowSeconds: 60);
    }
}
