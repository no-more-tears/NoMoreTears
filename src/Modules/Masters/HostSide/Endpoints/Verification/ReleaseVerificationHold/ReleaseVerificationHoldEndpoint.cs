namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.ReleaseVerificationHold;

public sealed class ReleaseVerificationHoldEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(Routes.Hold, ReleaseVerificationHoldHandler.Handle)
            .WithName(EndpointName.ReleaseVerificationHold)
            .WithTags(EndpointTag.Verification)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}
