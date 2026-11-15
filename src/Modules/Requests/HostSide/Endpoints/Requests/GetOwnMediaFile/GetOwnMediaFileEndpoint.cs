namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.GetOwnMediaFile;

public sealed class GetOwnMediaFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.MediaFile, GetOwnMediaFileHandler.Handle)
            .WithName(EndpointName.GetOwnMediaFile)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}