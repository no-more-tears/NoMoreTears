namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.GetMaster;

public sealed class GetMasterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Master, GetMasterHandler.Handle)
            .WithName(EndpointName.GetMaster)
            .WithTags(EndpointTag.Verification)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}
