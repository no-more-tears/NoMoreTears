namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.UpdateCategory;

public sealed class UpdateCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Routes.Category, UpdateCategoryHandler.Handle)
            .WithName(EndpointName.UpdateCategory)
            .WithTags(EndpointTag.Categories)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformAdministrator)));
    }
}
