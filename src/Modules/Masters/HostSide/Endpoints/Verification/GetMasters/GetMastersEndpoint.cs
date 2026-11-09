namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.GetMasters;

public sealed class GetMastersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Masters, GetMastersHandler.Handle)
            .WithName(EndpointName.GetMasters)
            .WithTags(EndpointTag.Verification)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}
