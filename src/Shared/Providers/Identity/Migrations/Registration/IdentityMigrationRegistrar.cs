using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Postgres.Migrations.Registration;
using NoMoreTears.Shared.Providers.Identity.Persistence;

namespace NoMoreTears.Shared.Providers.Identity.Migrations.Registration;

/// <summary>
///     Registrar for applying identity migrations from a migrations host.
/// </summary>
public static class IdentityMigrationRegistrar
{
    /// <param name="services">DI service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds the migration manager for <see cref="IdentityDbContext"/> with this assembly as the migrations host.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="connectionStringName">Name of the connection string in the configuration.</param>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddIdentityMigrationManager(
            IConfiguration configuration,
            string connectionStringName)
        {
            return services.AddMigrationManager<IdentityDbContext, IdentityDbContextFactory>(
                configuration,
                connectionStringName);
        }
    }
}
