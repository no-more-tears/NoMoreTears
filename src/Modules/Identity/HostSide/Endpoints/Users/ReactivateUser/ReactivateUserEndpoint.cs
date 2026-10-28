using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NoMoreTears.Modules.Identity.Contracts.Constants;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.ReactivateUser;

public sealed class ReactivateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Reactivate, ReactivateUserHandler.Handle)
            .WithName(EndpointName.ReactivateUser)
            .WithTags(EndpointTag.Users)
            .RequireAuthorization();
    }
}
