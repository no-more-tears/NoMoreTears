using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts;

namespace NoMoreTears.Modules.Requests.HostSide.Migrations.DesignTime;

/// <summary>
///     Creates <see cref="RequestsDbContext"/> for <c>dotnet ef</c>, so that migrations can be added
///     without a running database or host configuration.
/// </summary>
internal sealed class RequestsDesignTimeDbContextFactory : IDesignTimeDbContextFactory<RequestsDbContext>
{
    private const string DesignTimeConnectionString = "Host=localhost;Database=nomoretears";

    public RequestsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<RequestsDbContext>()
            .UseNpgsql(DesignTimeConnectionString, npgsql => npgsql
                .MigrationsAssembly(typeof(AssemblyReference).Assembly.FullName)
                .MigrationsHistoryTable(HistoryRepository.DefaultTableName, RequestsDbContext.Schema))
            .Options;

        return new RequestsDbContext(options);
    }
}