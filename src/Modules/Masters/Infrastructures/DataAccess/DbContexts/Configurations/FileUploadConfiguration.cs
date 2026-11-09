using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Modules.Masters.Domain;
using NoMoreTears.Modules.Masters.Domain.Aggregates;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="FileUpload"/>.
/// </summary>
public sealed class FileUploadConfiguration : IEntityTypeConfiguration<FileUpload>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<FileUpload> builder)
    {
        builder.ToTable("FileUploads");

        builder.ConfigureAuditableEntity<FileUpload, Guid>();
        builder.UseOptimisticConcurrency();

        builder.Property(upload => upload.MasterId).IsRequired();

        builder.Property(upload => upload.Purpose)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.Enum)
            .IsRequired();

        builder.Property(upload => upload.CategoryId);

        builder.Property(upload => upload.FileName)
            .HasMaxLength(MasterRules.MaxFileNameLength)
            .IsRequired();

        builder.Property(upload => upload.ContentType)
            .HasMaxLength(ColumnLengths.ContentType)
            .IsRequired();

        builder.Property(upload => upload.SizeInBytes).IsRequired();

        builder.Property(upload => upload.StagingKey)
            .HasMaxLength(MasterRules.MaxStorageKeyLength)
            .IsRequired();

        builder.Property(upload => upload.Status)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.Enum)
            .IsRequired();

        builder.Property(upload => upload.FailureReason)
            .HasConversion(
                reason => reason!.Value,
                value => Reason.Of(value))
            .HasMaxLength(MasterRules.MaxReasonLength);

        builder.Property(upload => upload.DocumentId);
        builder.Property(upload => upload.ProcessedAt);

        builder.HasIndex(upload => new { upload.MasterId, upload.Status });
    }
}