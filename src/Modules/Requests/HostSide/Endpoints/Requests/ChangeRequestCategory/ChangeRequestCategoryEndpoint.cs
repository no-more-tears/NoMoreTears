namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ChangeRequestCategory;

public sealed class ChangeRequestCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Routes.Category, ChangeRequestCategoryHandler.Handle)
            .WithName(EndpointName.ChangeRequestCategory)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}