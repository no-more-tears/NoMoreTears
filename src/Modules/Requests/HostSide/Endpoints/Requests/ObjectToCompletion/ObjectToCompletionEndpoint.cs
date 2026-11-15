namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ObjectToCompletion;

public sealed class ObjectToCompletionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.ObjectToCompletion, ObjectToCompletionHandler.Handle)
            .WithName(EndpointName.ObjectToCompletion)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}