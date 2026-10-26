using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Providers.Hosting;
using NoMoreTears.Shared.Providers.Identity.Registration;
using CommonIdentityRegistrar = NoMoreTears.Shared.Providers.Identity.Registration.IdentityRegistrar;

namespace NoMoreTears.Modules.Identity.Providers;

public sealed class IdentityModule : IModule
{
    private const string ConnectionStringName = "Postgres";

    public string Name => nameof(IdentityModule);

    public void Register(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddIdentityProvider(configuration, identity => identity
                .UsePostgres(blueprint => blueprint.UseConnectionString(ConnectionStringName))
                .UseJwtBearer())
            .AddApplications(configuration.GetSection(CommonIdentityRegistrar.SectionName))
            .AddEndpoints()
            .AddAuthorization();
    }
}