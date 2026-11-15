using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Modules.Requests.Domain;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Modules.Requests.Domain.ValueObjects;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="MediaUpload"/>.
/// </summary>
public sealed class MediaUploadConfiguration : IEntityTypeConfiguration<MediaUpload>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<MediaUpload> builder)
    {
        builder.ToTable("MediaUploads");

        builder.ConfigureAuditableEntity<MediaUpload, Guid>();
        builder.UseOptimisticConcurrency();

        builder.Property(upload => upload.RequestId).IsRequired();
        builder.Property(upload => upload.ClientId).IsRequired();

        builder.Property(upload => upload.FileName)
            .HasMaxLength(RequestRules.MaxFileNameLength)
            .IsRequired();

        builder.Property(upload => upload.ContentType)
            .HasMaxLength(ColumnLengths.ContentType)
            .IsRequired();

        builder.Property(upload => upload.SizeInBytes).IsRequired();

        builder.Property(upload => upload.StagingKey)
            .HasMaxLength(RequestRules.MaxStorageKeyLength)
            .IsRequired();

        builder.Property(upload => upload.Status)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.Enum)
            .IsRequired();

        builder.Property(upload => upload.FailureReason)
            .HasConversion(
                reason => reason!.Value,
                value => Reason.Of(value))
            .HasMaxLength(RequestRules.MaxReasonLength);

        builder.Property(upload => upload.MediaFileId);
        builder.Property(upload => upload.ProcessedAt);

        builder.HasIndex(upload => new { upload.RequestId, upload.Status });
    }
}