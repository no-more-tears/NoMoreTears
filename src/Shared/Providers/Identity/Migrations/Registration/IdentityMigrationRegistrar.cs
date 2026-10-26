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
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds the migration manager for <see cref="IdentityDbContext"/> with this assembly as the migrations host.
        /// </summary>
        public IServiceCollection AddIdentityMigrationManager<TAssembly>(
            IConfiguration configuration,
            string connectionStringName)
        {
            return services.AddMigrationManager<IdentityDbContext, TAssembly>(
                configuration,
                connectionStringName);
        }
    }
}