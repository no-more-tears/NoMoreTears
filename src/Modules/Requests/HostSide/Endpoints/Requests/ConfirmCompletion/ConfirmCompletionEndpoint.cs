namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ConfirmCompletion;

public sealed class ConfirmCompletionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.ConfirmCompletion, ConfirmCompletionHandler.Handle)
            .WithName(EndpointName.ConfirmCompletion)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}