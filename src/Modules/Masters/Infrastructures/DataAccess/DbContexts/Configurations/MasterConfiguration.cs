using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Modules.Masters.Domain;
using NoMoreTears.Modules.Masters.Domain.Aggregates;
using NoMoreTears.Modules.Masters.Domain.Entities;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;
using NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts.Conversions;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for the <see cref="Master"/> aggregate.
/// </summary>
public sealed class MasterConfiguration : IEntityTypeConfiguration<Master>
{
    private const string MasterIdColumn = "MasterId";

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Master> builder)
    {
        builder.ToTable("Masters");

        builder.ConfigureAuditableEntity<Master, Guid>();

        // Sanctions from Trust and edits by the master may arrive at the same time.
        builder.UseOptimisticConcurrency();

        ConfigureProfile(builder);
        ConfigureVerification(builder);
        ConfigureServiceArea(builder);
        ConfigureCategories(builder);
        ConfigureDocuments(builder);
        ConfigureSanctions(builder);

        builder.HasIndex(master => master.Verification);
        builder.HasIndex(master => master.IsAvailable);
    }

    private static void ConfigureProfile(EntityTypeBuilder<Master> builder)
    {
        builder.OwnsOne(master => master.FullName, fullName =>
        {
            fullName.Property(name => name.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(MasterRules.MaxNamePartLength)
                .IsRequired();

            fullName.Property(name => name.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(MasterRules.MaxNamePartLength)
                .IsRequired();
        });

        builder.Navigation(master => master.FullName).IsRequired();

        builder.OwnsOne(master => master.Avatar, avatar =>
        {
            avatar.Property(photo => photo.StorageKey)
                .HasColumnName("AvatarStorageKey")
                .HasMaxLength(MasterRules.MaxStorageKeyLength)
                .IsRequired();

            avatar.Property(photo => photo.ContentType)
                .HasColumnName("AvatarContentType")
                .HasMaxLength(ColumnLengths.ContentType)
                .IsRequired();

            avatar.Property(photo => photo.SizeInBytes)
                .HasColumnName("AvatarSizeInBytes")
                .IsRequired();
        });

        builder.Property(master => master.WorkSchedule)
            .HasConversion(
                schedule => ValueObjectJson.SerializeWorkSchedule(schedule),
                json => ValueObjectJson.DeserializeWorkSchedule(json))
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(master => master.IsAvailable).IsRequired();
    }

    private static void ConfigureVerification(EntityTypeBuilder<Master> builder)
    {
        builder.Property(master => master.Verification)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.Enum)
            .IsRequired();

        builder.OwnsOne(master => master.VerificationReview, review => ConfigureReview(review, "Verification"));

        builder.Property(master => master.SubmittedAt);
        builder.Property(master => master.VerifiedAt);
        builder.Property(master => master.RevokedAt);
        builder.Property(master => master.ReverificationAllowedFrom);

        builder.OwnsOne(master => master.VerificationHold, hold =>
        {
            hold.Property(value => value.Reason)
                .HasColumnName("VerificationHoldReason")
                .HasConversion(
                    reason => reason.Value,
                    value => Reason.Of(value))
                .HasMaxLength(MasterRules.MaxReasonLength)
                .IsRequired();

            hold.Property(value => value.PlacedAt)
                .HasColumnName("VerificationHoldPlacedAt")
                .IsRequired();
        });
    }

    private static void ConfigureServiceArea(EntityTypeBuilder<Master> builder)
    {
        builder.Property(master => master.ServiceArea)
            .HasConversion(
                area => ValueObjectJson.SerializeServiceArea(area),
                json => ValueObjectJson.DeserializeServiceArea(json))
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(master => master.ServiceAreaStatus)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.Enum)
            .IsRequired();

        builder.OwnsOne(master => master.ServiceAreaReview, review => ConfigureReview(review, "ServiceArea"));
    }

    private static void ConfigureCategories(EntityTypeBuilder<Master> builder)
    {
        builder.OwnsMany(master => master.Categories, categories =>
        {
            categories.ToTable("MasterCategories");

            categories.WithOwner().HasForeignKey(MasterIdColumn);
            categories.HasKey(MasterIdColumn, nameof(MasterCategory.CategoryId));

            categories.Property(category => category.CategoryId).ValueGeneratedNever();
            categories.Property(category => category.AddedAt).IsRequired();

            categories.Property(category => category.Status)
                .HasConversion<string>()
                .HasMaxLength(ColumnLengths.Enum)
                .IsRequired();

            categories.Ignore(category => category.IsVerified);

            categories.OwnsOne(category => category.Review, review => ConfigureReview(review, string.Empty));

            categories.HasIndex(category => category.CategoryId);
        });

        builder.Navigation(master => master.Categories).UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureDocuments(EntityTypeBuilder<Master> builder)
    {
        builder.OwnsMany(master => master.Documents, documents =>
        {
            documents.ToTable("VerificationDocuments");

            documents.WithOwner().HasForeignKey(MasterIdColumn);
            documents.HasKey(document => document.Id);

            documents.Property(document => document.Id).ValueGeneratedNever();

            documents.Property(document => document.Kind)
                .HasConversion<string>()
                .HasMaxLength(ColumnLengths.Enum)
                .IsRequired();

            documents.Property(document => document.CategoryId);

            documents.Property(document => document.StorageKey)
                .HasMaxLength(MasterRules.MaxStorageKeyLength)
                .IsRequired();

            documents.Property(document => document.ContentType)
                .HasMaxLength(ColumnLengths.ContentType)
                .IsRequired();

            documents.Property(document => document.SizeInBytes).IsRequired();
            documents.Property(document => document.UploadedAt).IsRequired();

            documents.HasIndex(MasterIdColumn);
        });

        builder.Navigation(master => master.Documents).UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureSanctions(EntityTypeBuilder<Master> builder)
    {
        builder.OwnsMany(master => master.Sanctions, sanctions =>
        {
            sanctions.ToTable("MasterSanctions");

            sanctions.WithOwner().HasForeignKey(MasterIdColumn);
            sanctions.HasKey(sanction => sanction.Id);

            // The identifier comes from Trust (sanction or temporary measure).
            sanctions.Property(sanction => sanction.Id).ValueGeneratedNever();

            sanctions.Property(sanction => sanction.Kind)
                .HasConversion<string>()
                .HasMaxLength(ColumnLengths.Enum)
                .IsRequired();

            sanctions.Property(sanction => sanction.ImposedAt).IsRequired();
            sanctions.Property(sanction => sanction.ExpiresAt);
            sanctions.Property(sanction => sanction.LiftedAt);

            sanctions.HasIndex(MasterIdColumn);
        });

        builder.Navigation(master => master.Sanctions).UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureReview<TOwner>(
        OwnedNavigationBuilder<TOwner, ReviewDecision> review,
        string columnPrefix)
        where TOwner : class
    {
        review.Property(decision => decision.ReviewerId)
            .HasColumnName($"{columnPrefix}ReviewerId")
            .IsRequired();

        review.Property(decision => decision.ReviewedAt)
            .HasColumnName($"{columnPrefix}ReviewedAt")
            .IsRequired();

        review.Property(decision => decision.Reason)
            .HasColumnName($"{columnPrefix}ReviewReason")
            .HasConversion(
                reason => reason!.Value,
                value => Reason.Of(value))
            .HasMaxLength(MasterRules.MaxReasonLength);
    }
}