using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NoMoreTears.Modules.Identity.Contracts.Constants;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.LogoutAll;

public sealed class LogoutAllEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.LogoutAll, LogoutAllHandler.Handle)
            .WithName(EndpointName.LogoutAll)
            .WithTags(EndpointTag.Identity)
            .RequireAuthorization();
    }
}
