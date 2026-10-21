using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

namespace NoMoreTears.Shared.Providers.Identity.Persistence.Configurations;

/// <summary>
///     Mapping of <see cref="UserEntity"/> to the <c>Users</c> table.
/// </summary>
internal sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(ColumnLengths.Email);

        builder
            .Property(x => x.NormalizedEmail)
            .IsRequired()
            .HasMaxLength(ColumnLengths.Email);

        builder
            .Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(ColumnLengths.Username);

        builder
            .Property(x => x.NormalizedUsername)
            .IsRequired()
            .HasMaxLength(ColumnLengths.Username);

        builder
            .Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(ColumnLengths.PasswordHash);

        builder
            .Property(x => x.EmailConfirmed)
            .HasDefaultValue(false);

        builder
            .Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder
            .Property(x => x.MfaEnabled)
            .HasDefaultValue(false);

        builder
            .Property(x => x.FailedAccessAttempts)
            .HasDefaultValue(0);

        builder
            .HasMany(x => x.Roles)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.NormalizedEmail)
            .IsUnique();
        builder
            .HasIndex(x => x.NormalizedUsername)
            .IsUnique();
    }
}