namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.AddMasterCategory;

public sealed class AddMasterCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Categories, AddMasterCategoryHandler.Handle)
            .WithName(EndpointName.AddMasterCategory)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
