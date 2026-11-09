using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NoMoreTears.Shared.Providers.Identity.Persistence;

namespace NoMoreTears.Shared.Providers.Identity.Migrations.DesignTime;

/// <summary>
///     Creates <see cref="IdentityDbContext"/> for <c>dotnet ef</c>, so that migrations can be added
///     without a running database or host configuration.
/// </summary>
internal sealed class IdentityDesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    private const string DesignTimeConnectionString = "Host=localhost;Database=nomoretears";

    public IdentityDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(DesignTimeConnectionString, npgsql => npgsql
                .MigrationsAssembly(typeof(AssemblyReference).Assembly.FullName))
            .Options;

        return new IdentityDbContext(options);
    }
}
