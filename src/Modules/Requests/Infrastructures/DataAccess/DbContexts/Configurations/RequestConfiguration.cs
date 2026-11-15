using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Modules.Requests.Domain;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Modules.Requests.Domain.Entities;
using NoMoreTears.Modules.Requests.Domain.ValueObjects;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for the <see cref="Request"/> aggregate.
/// </summary>
public sealed class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    private const string RequestIdColumn = "RequestId";

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.ToTable("Requests");

        builder.ConfigureAuditableEntity<Request, Guid>();

        builder.UseOptimisticConcurrency();

        builder.Property(request => request.ClientId).IsRequired();
        builder.Property(request => request.CategoryId).IsRequired();

        ConfigureContent(builder);
        ConfigureLifecycle(builder);
        ConfigureMediaFiles(builder);
        ConfigureStatusHistory(builder);

        builder.Ignore(request => request.IsRepublished);

        builder.HasIndex(request => new { request.ClientId, request.Status });
        builder.HasIndex(request => new { request.Status, request.AcceptanceDeadline });
        builder.HasIndex(request => new { request.Status, request.ConfirmationDeadline });
    }

    private static void ConfigureContent(EntityTypeBuilder<Request> builder)
    {
        builder.OwnsOne(request => request.Address, address =>
        {
            address.Property(value => value.Value)
                .HasColumnName("Address")
                .HasMaxLength(RequestRules.MaxAddressLength)
                .IsRequired();

            address.OwnsOne(value => value.Coordinates, coordinates =>
            {
                coordinates.Property(point => point.Latitude)
                    .HasColumnName("Latitude")
                    .IsRequired();

                coordinates.Property(point => point.Longitude)
                    .HasColumnName("Longitude")
                    .IsRequired();
            });

            address.Navigation(value => value.Coordinates).IsRequired();
        });

        builder.Navigation(request => request.Address).IsRequired();

        builder.Property(request => request.Description)
            .HasConversion(
                description => description.Value,
                value => Description.Of(value))
            .HasMaxLength(RequestRules.MaxDescriptionLength)
            .IsRequired();
    }

    private static void ConfigureLifecycle(EntityTypeBuilder<Request> builder)
    {
        builder.Property(request => request.Status)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.Enum)
            .IsRequired();

        builder.Property(request => request.PublicationCount).IsRequired();
        builder.Property(request => request.PublishedAt);
        builder.Property(request => request.AcceptanceDeadline);
        builder.Property(request => request.ConfirmationDeadline);
        builder.Property(request => request.CompletedAt);
        builder.Property(request => request.CancelledAt);

        builder.Property(request => request.CancellationReason)
            .HasConversion(
                reason => reason!.Value,
                value => Reason.Of(value))
            .HasMaxLength(RequestRules.MaxReasonLength);

        builder.Property(request => request.IncidentId);

        builder.Property(request => request.StatusBeforeDispute)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.Enum);

        builder.OwnsOne(request => request.Assignment, assignment =>
        {
            assignment.Property(value => value.MasterId)
                .HasColumnName("AssignedMasterId")
                .IsRequired();

            assignment.Property(value => value.AcceptedAt)
                .HasColumnName("AcceptedAt")
                .IsRequired();

            assignment.Property(value => value.ExpectedArrivalAt)
                .HasColumnName("ExpectedArrivalAt")
                .IsRequired();

            assignment.Ignore(value => value.NoShowReportableFrom);

            assignment.HasIndex(value => value.MasterId);
        });

        builder.OwnsOne(request => request.Failure, failure =>
        {
            failure.Property(value => value.Kind)
                .HasColumnName("FailureKind")
                .HasConversion<string>()
                .HasMaxLength(ColumnLengths.Enum)
                .IsRequired();

            failure.Property(value => value.MasterId)
                .HasColumnName("FailureMasterId")
                .IsRequired();

            failure.Property(value => value.Reason)
                .HasColumnName("FailureReason")
                .HasConversion(
                    reason => reason!.Value,
                    value => Reason.Of(value))
                .HasMaxLength(RequestRules.MaxReasonLength);
        });
    }

    private static void ConfigureMediaFiles(EntityTypeBuilder<Request> builder)
    {
        builder.OwnsMany(request => request.MediaFiles, mediaFiles =>
        {
            mediaFiles.ToTable("RequestMediaFiles");

            mediaFiles.WithOwner().HasForeignKey(RequestIdColumn);
            mediaFiles.HasKey(mediaFile => mediaFile.Id);

            mediaFiles.Property(mediaFile => mediaFile.Id).ValueGeneratedNever();

            mediaFiles.Property(mediaFile => mediaFile.StorageKey)
                .HasMaxLength(RequestRules.MaxStorageKeyLength)
                .IsRequired();

            mediaFiles.Property(mediaFile => mediaFile.ContentType)
                .HasMaxLength(ColumnLengths.ContentType)
                .IsRequired();

            mediaFiles.Property(mediaFile => mediaFile.SizeInBytes).IsRequired();
            mediaFiles.Property(mediaFile => mediaFile.AttachedAt).IsRequired();

            mediaFiles.HasIndex(RequestIdColumn);
        });

        builder.Navigation(request => request.MediaFiles).UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureStatusHistory(EntityTypeBuilder<Request> builder)
    {
        builder.OwnsMany(request => request.StatusHistory, history =>
        {
            history.ToTable("RequestStatusChanges");

            history.WithOwner().HasForeignKey(RequestIdColumn);
            history.HasKey(change => change.Id);

            history.Property(change => change.Id).ValueGeneratedNever();

            history.Property(change => change.From)
                .HasConversion<string>()
                .HasMaxLength(ColumnLengths.Enum);

            history.Property(change => change.To)
                .HasConversion<string>()
                .HasMaxLength(ColumnLengths.Enum)
                .IsRequired();

            history.Property(change => change.Transition)
                .HasConversion<string>()
                .HasMaxLength(ColumnLengths.Enum)
                .IsRequired();

            history.OwnsOne(change => change.Initiator, initiator =>
            {
                initiator.Property(value => value.Role)
                    .HasColumnName("InitiatorRole")
                    .HasConversion<string>()
                    .HasMaxLength(ColumnLengths.Enum)
                    .IsRequired();

                initiator.Property(value => value.UserId)
                    .HasColumnName("InitiatorId");
            });

            history.Navigation(change => change.Initiator).IsRequired();

            history.Property(change => change.Reason)
                .HasConversion(
                    reason => reason!.Value,
                    value => Reason.Of(value))
                .HasMaxLength(RequestRules.MaxReasonLength);

            history.Property(change => change.OccurredAt).IsRequired();

            history.HasIndex(RequestIdColumn, nameof(StatusChange.OccurredAt));
        });

        builder.Navigation(request => request.StatusHistory).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}