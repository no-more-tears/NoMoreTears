using Microsoft.EntityFrameworkCore;
using NoMoreTears.Modules.Notification.Infrastructures.DataAccess.DbContexts.Configurations;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Notification.Infrastructures.DataAccess.DbContexts;

/// <summary>
///     Centralizes model-building for <see cref="NotificationDbContext"/>.
/// </summary>
public static class CustomModelBuilder
{
    /// <summary>
    ///     Applies all entity type configurations and shared model conventions.
    /// </summary>
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(NotificationDbContext.Schema)
            .ApplyConfiguration(new NotificationConfiguration())
            .ApplyConfiguration(new EmailDeliveryConfiguration())
            .AddTransactionalOutbox()
            .ApplySoftDeleteQueryFilter();
    }
}