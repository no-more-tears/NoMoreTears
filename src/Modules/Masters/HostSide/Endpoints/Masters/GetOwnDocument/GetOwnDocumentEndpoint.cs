namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.GetOwnDocument;

public sealed class GetOwnDocumentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Document, GetOwnDocumentHandler.Handle)
            .WithName(EndpointName.GetOwnDocument)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
