using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NoMoreTears.Modules.Identity.Contracts.Constants;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.DeactivateUser;

public sealed class DeactivateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Deactivate, DeactivateUserHandler.Handle)
            .WithName(EndpointName.DeactivateUser)
            .WithTags(EndpointTag.Users)
            .RequireAuthorization();
    }
}
