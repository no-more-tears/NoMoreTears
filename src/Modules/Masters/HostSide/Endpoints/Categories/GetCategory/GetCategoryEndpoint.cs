namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.GetCategory;

public sealed class GetCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Category, GetCategoryHandler.Handle)
            .WithName(EndpointName.GetCategory)
            .WithTags(EndpointTag.Categories)
            .AllowAnonymous();
    }
}
