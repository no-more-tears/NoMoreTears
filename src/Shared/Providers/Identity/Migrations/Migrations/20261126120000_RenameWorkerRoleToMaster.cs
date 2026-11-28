using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoMoreTears.Shared.Providers.Identity.Migrations.Migrations
{
    /// <summary>
    ///     The Worker role is renamed to Master. Roles are stored by name, so existing rows are renamed;
    ///     the model itself does not change.
    /// </summary>
    public partial class RenameWorkerRoleToMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""UPDATE identity."UserRoles" SET "Role" = 'Master' WHERE "Role" = 'Worker';""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""UPDATE identity."UserRoles" SET "Role" = 'Worker' WHERE "Role" = 'Master';""");
        }
    }
}