namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.VerifyMasterCategory;

public sealed class VerifyMasterCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.VerifyCategory, VerifyMasterCategoryHandler.Handle)
            .WithName(EndpointName.VerifyMasterCategory)
            .WithTags(EndpointTag.Verification)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}
