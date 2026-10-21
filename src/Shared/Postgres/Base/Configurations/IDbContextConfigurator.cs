using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace NoMoreTears.Shared.Postgres.Configurations;

/// <summary>
///     Defines a contract for configuring a DbContext.
/// </summary>
/// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
public interface IDbContextConfigurator<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    ///     Configures the DbContext options.
    /// </summary>
    void Configure(DbContextOptionsBuilder<TDbContext> optionsBuilder);

    /// <summary>
    ///    Configures the DbContext options with additional Npgsql-specific options.
    /// </summary>
    void Configure(
        DbContextOptionsBuilder<TDbContext> optionsBuilder,
        Action<NpgsqlDbContextOptionsBuilder> optionsAction);
}