namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.ArchiveCategory;

public sealed class ArchiveCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Archive, ArchiveCategoryHandler.Handle)
            .WithName(EndpointName.ArchiveCategory)
            .WithTags(EndpointTag.Categories)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformAdministrator)));
    }
}
