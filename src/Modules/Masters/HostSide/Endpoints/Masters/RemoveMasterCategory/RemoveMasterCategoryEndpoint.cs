namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.RemoveMasterCategory;

public sealed class RemoveMasterCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(Routes.Category, RemoveMasterCategoryHandler.Handle)
            .WithName(EndpointName.RemoveMasterCategory)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
