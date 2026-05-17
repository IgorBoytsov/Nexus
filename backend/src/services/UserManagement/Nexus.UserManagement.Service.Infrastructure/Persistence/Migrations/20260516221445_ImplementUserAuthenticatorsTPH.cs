using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImplementUserAuthenticatorsTPH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "UserAuthenticators");

            migrationBuilder.RenameColumn(
                name: "Salt",
                table: "UserAuthenticators",
                newName: "SrpSalt");

            migrationBuilder.RenameColumn(
                name: "CredentialData",
                table: "UserAuthenticators",
                newName: "SrpVerificator");

            migrationBuilder.AlterColumn<string>(
                name: "SrpSalt",
                table: "UserAuthenticators",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserAuthenticators",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserAuthenticators",
                type: "text",
                nullable: true,
                collation: "case_insensitive");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserAuthenticators",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAt",
                table: "UserAuthenticators",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SrpLogin",
                table: "UserAuthenticators",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserAuthenticators");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserAuthenticators");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserAuthenticators");

            migrationBuilder.DropColumn(
                name: "LastUsedAt",
                table: "UserAuthenticators");

            migrationBuilder.DropColumn(
                name: "SrpLogin",
                table: "UserAuthenticators");

            migrationBuilder.RenameColumn(
                name: "SrpSalt",
                table: "UserAuthenticators",
                newName: "Salt");

            migrationBuilder.RenameColumn(
                name: "SrpVerificator",
                table: "UserAuthenticators",
                newName: "CredentialData");

            migrationBuilder.AlterColumn<string>(
                name: "Salt",
                table: "UserAuthenticators",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "UserAuthenticators",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
