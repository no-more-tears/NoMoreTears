namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Support.GetRequestMediaFile;

public sealed class GetRequestMediaFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.MediaFile, GetRequestMediaFileHandler.Handle)
            .WithName(EndpointName.GetRequestMediaFile)
            .WithTags(EndpointTag.Support)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}