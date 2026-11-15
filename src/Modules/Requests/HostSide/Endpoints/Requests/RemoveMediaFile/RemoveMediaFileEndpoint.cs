namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.RemoveMediaFile;

public sealed class RemoveMediaFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(Routes.MediaFile, RemoveMediaFileHandler.Handle)
            .WithName(EndpointName.RemoveMediaFile)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}