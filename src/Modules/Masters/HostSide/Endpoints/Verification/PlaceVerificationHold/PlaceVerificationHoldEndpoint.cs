namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.PlaceVerificationHold;

public sealed class PlaceVerificationHoldEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Hold, PlaceVerificationHoldHandler.Handle)
            .WithName(EndpointName.PlaceVerificationHold)
            .WithTags(EndpointTag.Verification)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}
