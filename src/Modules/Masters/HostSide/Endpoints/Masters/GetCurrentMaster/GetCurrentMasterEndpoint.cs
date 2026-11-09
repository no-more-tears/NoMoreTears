namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.GetCurrentMaster;

public sealed class GetCurrentMasterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Me, GetCurrentMasterHandler.Handle)
            .WithName(EndpointName.GetCurrentMaster)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
