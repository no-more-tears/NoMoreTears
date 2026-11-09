namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.RemoveDocument;

public sealed class RemoveDocumentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(Routes.Document, RemoveDocumentHandler.Handle)
            .WithName(EndpointName.RemoveDocument)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
