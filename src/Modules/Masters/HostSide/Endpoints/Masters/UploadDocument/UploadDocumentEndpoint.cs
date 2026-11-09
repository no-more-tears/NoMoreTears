namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.UploadDocument;

public sealed class UploadDocumentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Documents, UploadDocumentHandler.Handle)
            .WithName(EndpointName.UploadDocument)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)))
            .DisableAntiforgery()
            .WithMetadata(new RequestSizeLimitAttribute(UploadLimits.DocumentRequestSizeInBytes));
    }
}
