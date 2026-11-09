namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.RejectMasterCategory;

public sealed class RejectMasterCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.RejectCategory, RejectMasterCategoryHandler.Handle)
            .WithName(EndpointName.RejectMasterCategory)
            .WithTags(EndpointTag.Verification)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}
