using Microsoft.Extensions.DependencyInjection;

namespace NoMoreTears.Shared.Common.Authorization.Registration;

public static class AuthorizationRegistrar
{
    public static IServiceCollection AddUserSessions(
       this IServiceCollection services,
       Func<IServiceProvider, ICurrentUserSession> factory)
    {
        services.AddScoped(factory);
        return services;
    }
}