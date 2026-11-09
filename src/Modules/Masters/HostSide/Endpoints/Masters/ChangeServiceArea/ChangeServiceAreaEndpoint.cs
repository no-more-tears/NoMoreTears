namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.ChangeServiceArea;

public sealed class ChangeServiceAreaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Routes.ServiceArea, ChangeServiceAreaHandler.Handle)
            .WithName(EndpointName.ChangeServiceArea)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
