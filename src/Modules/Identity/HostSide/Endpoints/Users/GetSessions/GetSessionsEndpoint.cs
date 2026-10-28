using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NoMoreTears.Modules.Identity.Contracts.Constants;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.GetSessions;

public sealed class GetSessionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Sessions, GetSessionsHandler.Handle)
            .WithName(EndpointName.GetSessions)
            .WithTags(EndpointTag.Users)
            .RequireAuthorization();
    }
}
