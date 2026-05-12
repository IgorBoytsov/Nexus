using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Mirgrations
{
    /// <inheritdoc />
    public partial class Replace_EncryptionMetadata_On_CryptoVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "UserSecurityAssets");

            migrationBuilder.AddColumn<int>(
                name: "CryptoVersion",
                table: "UserSecurityAssets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CryptoVersion",
                table: "UserSecurityAssets");

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "UserSecurityAssets",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
