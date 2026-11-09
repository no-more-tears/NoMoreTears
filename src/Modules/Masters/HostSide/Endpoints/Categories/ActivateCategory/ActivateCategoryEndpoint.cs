namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.ActivateCategory;

public sealed class ActivateCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Activate, ActivateCategoryHandler.Handle)
            .WithName(EndpointName.ActivateCategory)
            .WithTags(EndpointTag.Categories)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformAdministrator)));
    }
}
