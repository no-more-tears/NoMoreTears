namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.RejectServiceArea;

public sealed class RejectServiceAreaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.RejectServiceArea, RejectServiceAreaHandler.Handle)
            .WithName(EndpointName.RejectServiceArea)
            .WithTags(EndpointTag.Verification)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}
