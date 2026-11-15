namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.UploadMediaFile;

public sealed class UploadMediaFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Media, UploadMediaFileHandler.Handle)
            .WithName(EndpointName.UploadMediaFile)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)))
            .DisableAntiforgery()
            .WithMetadata(new RequestSizeLimitAttribute(UploadLimits.MediaRequestSizeInBytes));
    }
}