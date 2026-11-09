namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.VerifyServiceArea;

public sealed class VerifyServiceAreaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.VerifyServiceArea, VerifyServiceAreaHandler.Handle)
            .WithName(EndpointName.VerifyServiceArea)
            .WithTags(EndpointTag.Verification)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}
