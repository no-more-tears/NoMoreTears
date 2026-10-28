using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NoMoreTears.Modules.Identity.Contracts.Constants;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.UpdateRoles;

public sealed class UpdateRolesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Routes.Roles, UpdateRolesHandler.Handle)
            .WithName(EndpointName.UpdateRoles)
            .WithTags(EndpointTag.Users)
            .RequireAuthorization();
    }
}
