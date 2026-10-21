using NoMoreTears.Modules.Identity.Providers;
using NoMoreTears.Shared.Providers.Hosting;

namespace NoMoreTears.HostSide.WebApi.Registration;

public static class HostRegistrar
{
    public static IServiceCollection AddHostModules(
        this IServiceCollection services, 
        IConfiguration configuration) =>
        services.AddModules(
            configuration,
            new IdentityModule());
}