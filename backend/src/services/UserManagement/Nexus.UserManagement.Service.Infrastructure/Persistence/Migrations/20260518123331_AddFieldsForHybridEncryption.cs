using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsForHybridEncryption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SrpVerificator",
                table: "UserAuthenticators",
                newName: "SrpEncryptedVerifierWrapKey");

            migrationBuilder.AddColumn<string>(
                name: "SrpAsymmetricKeyId",
                table: "UserAuthenticators",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SrpEncryptedVerifier",
                table: "UserAuthenticators",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SrpKeyWrapVersion",
                table: "UserAuthenticators",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SrpVersion",
                table: "UserAuthenticators",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SrpAsymmetricKeyId",
                table: "UserAuthenticators");

            migrationBuilder.DropColumn(
                name: "SrpEncryptedVerifier",
                table: "UserAuthenticators");

            migrationBuilder.DropColumn(
                name: "SrpKeyWrapVersion",
                table: "UserAuthenticators");

            migrationBuilder.DropColumn(
                name: "SrpVersion",
                table: "UserAuthenticators");

            migrationBuilder.RenameColumn(
                name: "SrpEncryptedVerifierWrapKey",
                table: "UserAuthenticators",
                newName: "SrpVerificator");
        }
    }
}
