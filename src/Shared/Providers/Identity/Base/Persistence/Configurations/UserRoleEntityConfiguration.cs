using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

namespace NoMoreTears.Shared.Providers.Identity.Persistence.Configurations;

/// <summary>
///     Mapping of <see cref="UserRoleEntity"/> to the <c>UserRoles</c> table.
/// </summary>
internal sealed class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(x => new { x.UserId, x.Role });

        builder
            .Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.EnumName);
    }
}