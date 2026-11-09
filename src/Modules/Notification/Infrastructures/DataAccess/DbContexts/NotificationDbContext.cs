using Microsoft.EntityFrameworkCore;
using NoMoreTears.Modules.Notification.Domain.Aggregates;
using NotificationModel = NoMoreTears.Modules.Notification.Domain.Aggregates.Notification;

namespace NoMoreTears.Modules.Notification.Infrastructures.DataAccess.DbContexts;

/// <summary>
///     EF Core database context for the Notification module.
/// </summary>
public sealed class NotificationDbContext(
    DbContextOptions<NotificationDbContext> options)
    : DbContext(options)
{
    /// <summary>
    ///     Database schema.
    /// </summary>
    public const string Schema = "notification";

    /// <summary>
    ///     In-app notifications.
    /// </summary>
    public DbSet<NotificationModel> Notifications => Set<NotificationModel>();

    /// <summary>
    ///     Delivery statuses of emails.
    /// </summary>
    public DbSet<EmailDelivery> EmailDeliveries => Set<EmailDelivery>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        CustomModelBuilder.Configure(modelBuilder);
    }
}