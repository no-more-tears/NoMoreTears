using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using NoMoreTears.Modules.Notification.Infrastructures.DataAccess.DbContexts;

namespace NoMoreTears.Modules.Notification.HostSide.Migrations.DesignTime;

/// <summary>
///     Creates <see cref="NotificationDbContext"/> for <c>dotnet ef</c>, so that migrations can be added
///     without a running database or host configuration.
/// </summary>
internal sealed class NotificationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
{
    private const string DesignTimeConnectionString = "Host=localhost;Database=nomoretears";

    public NotificationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseNpgsql(DesignTimeConnectionString, npgsql => npgsql
                .MigrationsAssembly(typeof(AssemblyReference).Assembly.FullName)
                .MigrationsHistoryTable(HistoryRepository.DefaultTableName, NotificationDbContext.Schema))
            .Options;

        return new NotificationDbContext(options);
    }
}