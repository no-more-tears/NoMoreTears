using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts;

namespace NoMoreTears.Modules.Masters.HostSide.Migrations.DesignTime;

/// <summary>
///     Creates <see cref="MastersDbContext"/> for <c>dotnet ef</c>, so that migrations can be added
///     without a running database or host configuration.
/// </summary>
internal sealed class MastersDesignTimeDbContextFactory : IDesignTimeDbContextFactory<MastersDbContext>
{
    private const string DesignTimeConnectionString = "Host=localhost;Database=nomoretears";

    public MastersDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MastersDbContext>()
            .UseNpgsql(DesignTimeConnectionString, npgsql => npgsql
                .MigrationsAssembly(typeof(AssemblyReference).Assembly.FullName)
                .MigrationsHistoryTable(HistoryRepository.DefaultTableName, MastersDbContext.Schema))
            .Options;

        return new MastersDbContext(options);
    }
}
