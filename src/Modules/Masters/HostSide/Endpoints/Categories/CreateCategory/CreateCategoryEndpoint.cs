namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.CreateCategory;

public sealed class CreateCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Base, CreateCategoryHandler.Handle)
            .WithName(EndpointName.CreateCategory)
            .WithTags(EndpointTag.Categories)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformAdministrator)));
    }
}
