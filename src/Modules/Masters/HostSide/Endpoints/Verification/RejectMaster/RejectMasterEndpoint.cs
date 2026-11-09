namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.RejectMaster;

public sealed class RejectMasterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Reject, RejectMasterHandler.Handle)
            .WithName(EndpointName.RejectMaster)
            .WithTags(EndpointTag.Verification)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}
