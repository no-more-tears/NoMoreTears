namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.VerifyMaster;

public sealed class VerifyMasterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Verify, VerifyMasterHandler.Handle)
            .WithName(EndpointName.VerifyMaster)
            .WithTags(EndpointTag.Verification)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}
