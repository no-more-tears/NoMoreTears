namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.GetMediaUpload;

public sealed class GetMediaUploadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Upload, GetMediaUploadHandler.Handle)
            .WithName(EndpointName.GetMediaUpload)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}