using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.Authentication.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRefreshTokenToRefreshTokenHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "AccessData");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "AccessData",
                newName: "RefreshTokenHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenHash",
                table: "AccessData",
                newName: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "AccessData",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
