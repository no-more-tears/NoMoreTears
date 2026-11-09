namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories.GetCategories;

public sealed class GetCategoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Base, GetCategoriesHandler.Handle)
            .WithName(EndpointName.GetCategories)
            .WithTags(EndpointTag.Categories)
            .AllowAnonymous();
    }
}
