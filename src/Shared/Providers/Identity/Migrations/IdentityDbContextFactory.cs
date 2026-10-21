using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NoMoreTears.Shared.Providers.Identity.Persistence;

namespace NoMoreTears.Shared.Providers.Identity.Migrations;

/// <summary>
///     Design-time factory used by the EF Core tooling to scaffold and apply migrations
///     for <see cref="IdentityDbContext"/>. It also marks this assembly as the migrations assembly.
/// </summary>
/// <remarks>
///     Scaffolding (<c>dotnet ef migrations add</c>) does not open a connection, so the placeholder
///     connection string is sufficient. Applying migrations from the tooling requires the
///     <see cref="ConnectionStringVariable"/> environment variable.
/// </remarks>
public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    /// <summary>
    ///     Environment variable holding the connection string for design-time operations.
    /// </summary>
    public const string ConnectionStringVariable = "IDENTITY_MIGRATIONS_CONNECTION_STRING";

    private const string PlaceholderConnectionString = "Host=localhost;Database=identity_design_time";

    /// <inheritdoc />
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable)
            ?? PlaceholderConnectionString;

        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(typeof(IdentityDbContextFactory).Assembly.FullName))
            .Options;

        return new IdentityDbContext(options);
    }
}
