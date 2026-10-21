using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

namespace NoMoreTears.Shared.Providers.Identity.Persistence.Configurations;

/// <summary>
///     Mapping of <see cref="MfaChallengeEntity"/> to the <c>MfaChallenges</c> table.
/// </summary>
internal sealed class MfaChallengeEntityConfiguration : IEntityTypeConfiguration<MfaChallengeEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<MfaChallengeEntity> builder)
    {
        builder.ToTable("MfaChallenges");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .Property(x => x.ChallengeHash)
            .IsRequired()
            .HasMaxLength(ColumnLengths.Hash);

        builder.Property(x => x.CodeHash)
            .IsRequired()
            .HasMaxLength(ColumnLengths.Hash);

        builder
            .Property(x => x.ClientFingerprint)
            .IsRequired()
            .HasMaxLength(ColumnLengths.Hash);

        builder
            .Property(x => x.FailedAttempts)
            .HasDefaultValue(0);

        builder
            .HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.ChallengeHash)
            .IsUnique();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ExpiresAt);
    }
}