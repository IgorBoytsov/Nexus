using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Replacing_PasswordHash_Verifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "UserCredentials");

            migrationBuilder.AlterColumn<string>(
                name: "ClientSalt",
                table: "UserCredentials",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Verifier",
                table: "UserCredentials",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Verifier",
                table: "UserCredentials");

            migrationBuilder.AlterColumn<string>(
                name: "ClientSalt",
                table: "UserCredentials",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "UserCredentials",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
