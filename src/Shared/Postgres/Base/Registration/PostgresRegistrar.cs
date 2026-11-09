using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;
using NoMoreTears.Shared.Postgres.Abstractions.Transactions;
using NoMoreTears.Shared.Postgres.Configurations;
using NoMoreTears.Shared.Postgres.Registration.Configuration;
using NoMoreTears.Shared.Postgres.Repositories;
using NoMoreTears.Shared.Postgres.Transactions;

namespace NoMoreTears.Shared.Postgres.Registration;

/// <summary>
///     Registrar for setting up data access with Postgres.
/// </summary>
public static class PostgresRegistrar
{
    /// <summary>
    ///     DbContext default pool size.
    /// </summary>
    public const int DbContextPoolSize = 128;

    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds Postgres configuration with a default pool size and a default repository.
        /// </summary>
        /// <remarks>
        ///     This method is recommended almost always.
        /// </remarks>
        public IServiceCollection AddPostgres<TDbContext, TDbContextConfigurator>()
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextConfigurator<TDbContext>
        {
            services
                .AddPostgresBase<TDbContext, TDbContextConfigurator>()
                .AddScoped(typeof(IRepository<,>), typeof(DefaultRepository<,>));

            return services;
        }

        /// <summary>
        ///     Adds Postgres configuration with a specified pool size and a default repository.
        /// </summary>
        public IServiceCollection AddPostgres<TDbContext, TDbContextConfigurator>(int poolSize)
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextConfigurator<TDbContext>
        {
            services
                .AddPostgresBase<TDbContext, TDbContextConfigurator>(poolSize)
                .AddScoped(typeof(IRepository<,>), typeof(DefaultRepository<,>));

            return services;
        }

        /// <summary>
        ///     Adds Postgres configuration with a default pool size and a specified repository.
        /// </summary>
        public IServiceCollection AddPostgres<TDbContext, TDbContextConfigurator>(
            Action<IServiceCollection> configureRepository)
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextConfigurator<TDbContext>
        {
            services.AddPostgresBase<TDbContext, TDbContextConfigurator>();
            configureRepository(services);

            return services;
        }

        /// <summary>
        ///      Adds Postgres configuration with a specified pool size and repository.
        /// </summary>
        public IServiceCollection AddPostgres<TDbContext, TDbContextConfigurator>(
            int poolSize,
            Action<IServiceCollection> configureRepository)
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextConfigurator<TDbContext>
        {
            services.AddPostgresBase<TDbContext, TDbContextConfigurator>(poolSize);
            configureRepository(services);

            return services;
        }

        /// <summary>
        ///     Adds Postgres configuration for a module of a modular monolith.
        /// </summary>
        /// <remarks>
        ///     Unlike <c>AddPostgres</c>, no non-generic <see cref="DbContext"/>, open generic repository
        ///     or global transactional scope is registered: repositories and the unit of work are bound
        ///     to <typeparamref name="TDbContext"/> explicitly, so several module contexts can share one host.
        /// </remarks>
        public IServiceCollection AddModulePostgres<TDbContext, TDbContextConfigurator>(
            Action<PostgresModuleBuilder<TDbContext>> configure,
            int poolSize = DbContextPoolSize)
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextConfigurator<TDbContext>
        {
            ArgumentNullException.ThrowIfNull(configure);

            services
                .AddDbContextPool<TDbContext>(ConfigureDbContext<TDbContext>, poolSize)
                .AddSingleton<IDbContextConfigurator<TDbContext>, TDbContextConfigurator>();

            configure(new PostgresModuleBuilder<TDbContext>(services));

            return services;
        }

        /// <summary>
        ///     Binds the commands of the given handlers assembly to a unit of work over <typeparamref name="TDbContext"/>.
        ///     The CQS transactional behavior resolves the scope by the assembly of the command.
        /// </summary>
        public IServiceCollection AddTransactionalScope<TDbContext>(
            Assembly handlersAssembly,
            TransactionMode mode = TransactionMode.Transaction)
            where TDbContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(handlersAssembly);

            return services.AddKeyedScoped<ITransactionalScope>(
                handlersAssembly,
                (provider, _) => new DefaultTransactionalScope(provider.GetRequiredService<TDbContext>(), mode));
        }

        /// <summary>
        ///     Adds the base Postgres configuration.
        /// </summary>
        private IServiceCollection AddPostgresBase<TDbContext, TDbContextConfigurator>(
            int poolSize = DbContextPoolSize)
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextConfigurator<TDbContext>
        {
            return services
                .AddDbContextPool<TDbContext>(ConfigureDbContext<TDbContext>, poolSize)
                .AddSingleton<IDbContextConfigurator<TDbContext>, TDbContextConfigurator>()
                .AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>());
        }
    }

    /// <summary>
    ///     Configures the DbContext using the provided configurator.
    /// </summary>
    private static void ConfigureDbContext<TDbContext>(
        IServiceProvider provider,
        DbContextOptionsBuilder builder)
        where TDbContext : DbContext
    {
        var configurator = provider.GetRequiredService<IDbContextConfigurator<TDbContext>>();
        configurator.Configure((DbContextOptionsBuilder<TDbContext>)builder);
    }
}