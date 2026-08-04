using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSrpCryptoVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Method",
                table: "user_authenticators",
                newName: "method");

            migrationBuilder.AddColumn<int>(
                name: "srp_crypto_version",
                table: "user_authenticators",
                type: "integer",
                nullable: true);
                
            migrationBuilder.Sql(
                @"UPDATE user_authenticators 
                  SET srp_crypto_version = 1 
                  WHERE method = 1 
                    AND srp_crypto_version IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "srp_crypto_version",
                table: "user_authenticators");

            migrationBuilder.RenameColumn(
                name: "method",
                table: "user_authenticators",
                newName: "Method");
        }
    }
}
