namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification.GetMasterDocument;

public sealed class GetMasterDocumentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Document, GetMasterDocumentHandler.Handle)
            .WithName(EndpointName.GetMasterDocument)
            .WithTags(EndpointTag.Verification)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}
