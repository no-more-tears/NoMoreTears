using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;
using NoMoreTears.Shared.Postgres.Abstractions.Transactions;
using NoMoreTears.Shared.Postgres.Repositories;

namespace NoMoreTears.Shared.Postgres.Registration.Configuration;

/// <summary>
///     Registers data access services that are bound to one <typeparamref name="TDbContext"/>,
///     so that several modules with their own contexts can live in one host.
/// </summary>
/// <typeparam name="TDbContext">The module DbContext.</typeparam>
public sealed class PostgresModuleBuilder<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceCollection _services;

    internal PostgresModuleBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    ///     Registers <see cref="IRepository{TEntity, TId}"/> for an entity of the module context.
    /// </summary>
    public PostgresModuleBuilder<TDbContext> AddRepository<TEntity, TId>()
        where TEntity : class
        where TId : struct
    {
        _services.AddScoped<IRepository<TEntity, TId>>(provider =>
            new DefaultRepository<TEntity, TId>(provider.GetRequiredService<TDbContext>()));

        return this;
    }

    /// <summary>
    ///     Binds the commands of the given handlers assembly to a unit of work over the module context.
    /// </summary>
    public PostgresModuleBuilder<TDbContext> AddTransactionalScope(
        Assembly handlersAssembly,
        TransactionMode mode = TransactionMode.Transaction)
    {
        _services.AddTransactionalScope<TDbContext>(handlersAssembly, mode);
        return this;
    }
}