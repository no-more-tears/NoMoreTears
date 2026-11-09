using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Modules.Notification.Domain;
using NoMoreTears.Modules.Notification.Domain.Aggregates;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Notification.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="EmailDelivery"/>.
/// </summary>
public sealed class EmailDeliveryConfiguration : IEntityTypeConfiguration<EmailDelivery>
{
    private const int EnumLength = 32;

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EmailDelivery> builder)
    {
        builder.ToTable("EmailDeliveries");

        builder.ConfigureAuditableEntity<EmailDelivery, Guid>();

        builder.Property(delivery => delivery.SourceEventId).IsRequired();
        builder.Property(delivery => delivery.RecipientId).IsRequired();

        builder.Property(delivery => delivery.Template)
            .HasMaxLength(NotificationRules.MaxTemplateLength)
            .IsRequired();

        builder.Property(delivery => delivery.Status)
            .HasConversion<string>()
            .HasMaxLength(EnumLength)
            .IsRequired();

        builder.Property(delivery => delivery.Attempts).IsRequired();

        builder.Property(delivery => delivery.LastError)
            .HasMaxLength(NotificationRules.MaxErrorLength);

        builder.Property(delivery => delivery.CompletedAt).IsRequired();

        // One event leads to at most one delivery.
        builder.HasIndex(delivery => delivery.SourceEventId).IsUnique();
        builder.HasIndex(delivery => new { delivery.RecipientId, delivery.CompletedAt });
    }
}
