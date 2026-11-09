using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Modules.Notification.Domain;
using NoMoreTears.Shared.Postgres.Extensions;
using NotificationModel = NoMoreTears.Modules.Notification.Domain.Aggregates.Notification;

namespace NoMoreTears.Modules.Notification.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="NotificationModel"/>.
/// </summary>
public sealed class NotificationConfiguration : IEntityTypeConfiguration<NotificationModel>
{
    private const int EnumLength = 32;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<NotificationModel> builder)
    {
        builder.ToTable("Notifications");

        builder.ConfigureAuditableEntity<NotificationModel, Guid>();

        builder.Property(notification => notification.RecipientId).IsRequired();
        builder.Property(notification => notification.SourceEventId).IsRequired();

        builder.Property(notification => notification.Type)
            .HasMaxLength(NotificationRules.MaxTypeLength)
            .IsRequired();

        builder.Property(notification => notification.Category)
            .HasConversion<string>()
            .HasMaxLength(EnumLength)
            .IsRequired();

        builder.Property(notification => notification.Severity)
            .HasConversion<string>()
            .HasMaxLength(EnumLength)
            .IsRequired();

        builder.Property(notification => notification.Title)
            .HasMaxLength(NotificationRules.MaxTitleLength)
            .IsRequired();

        builder.Property(notification => notification.Body)
            .HasMaxLength(NotificationRules.MaxBodyLength)
            .IsRequired();

        builder.Property(notification => notification.Data)
            .HasConversion(
                data => Serialize(data),
                json => Deserialize(json),
                new ValueComparer<IReadOnlyDictionary<string, string>>(
                    (left, right) => Serialize(left!) == Serialize(right!),
                    data => Serialize(data).GetHashCode(StringComparison.Ordinal),
                    data => data))
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(notification => notification.Link)
            .HasMaxLength(NotificationRules.MaxLinkLength);

        builder.Property(notification => notification.OccurredAt).IsRequired();
        builder.Property(notification => notification.DeliveredAt);
        builder.Property(notification => notification.ReadAt);

        builder.Ignore(notification => notification.IsRead);

        // One event of another module creates at most one notification per recipient.
        builder.HasIndex(notification => new { notification.SourceEventId, notification.RecipientId })
            .IsUnique();

        // The list of a user, newest first; partial indexes keep the unread and undelivered lookups small.
        builder.HasIndex(notification => new { notification.RecipientId, notification.CreatedAt });

        builder.HasIndex(notification => notification.RecipientId)
            .HasDatabaseName("IX_Notifications_RecipientId_Unread")
            .HasFilter($"\"{nameof(NotificationModel.ReadAt)}\" IS NULL AND \"{nameof(NotificationModel.IsDeleted)}\" = false");

        builder.HasIndex(notification => notification.RecipientId)
            .HasDatabaseName("IX_Notifications_RecipientId_Undelivered")
            .HasFilter($"\"{nameof(NotificationModel.DeliveredAt)}\" IS NULL AND \"{nameof(NotificationModel.IsDeleted)}\" = false");
    }

    private static string Serialize(IReadOnlyDictionary<string, string> data) =>
        JsonSerializer.Serialize(data, JsonOptions);

    private static Dictionary<string, string> Deserialize(string json) =>
        JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions) ?? [];
}
