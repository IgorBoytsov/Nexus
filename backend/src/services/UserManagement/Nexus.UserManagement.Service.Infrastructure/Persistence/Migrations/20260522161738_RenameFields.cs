using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAuthenticators_Users_UserId",
                table: "UserAuthenticators");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Countries_IdCountry",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Genders_IdGender",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Statuses_IdStatus",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserSecurityAssets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdCountry",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdGender",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdStatus",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Genders",
                table: "Genders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAuthenticators",
                table: "UserAuthenticators");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Statuses",
                newName: "statuses");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "roles");

            migrationBuilder.RenameTable(
                name: "Genders",
                newName: "genders");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "countries");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "user_roles");

            migrationBuilder.RenameTable(
                name: "UserAuthenticators",
                newName: "user_authenticators");

            migrationBuilder.RenameColumn(
                name: "Login",
                table: "users",
                newName: "login");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "users",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "IdStatus",
                table: "users",
                newName: "id_status");

            migrationBuilder.RenameColumn(
                name: "IdGender",
                table: "users",
                newName: "id_gender");

            migrationBuilder.RenameColumn(
                name: "IdCountry",
                table: "users",
                newName: "id_country");

            migrationBuilder.RenameColumn(
                name: "DateUpdate",
                table: "users",
                newName: "date_update");

            migrationBuilder.RenameColumn(
                name: "DateRegistration",
                table: "users",
                newName: "date_registration");

            migrationBuilder.RenameColumn(
                name: "DateEntry",
                table: "users",
                newName: "date_entry");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "statuses",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "statuses",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "roles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "roles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "genders",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "genders",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "countries",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "countries",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "user_roles",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_roles",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                table: "user_roles",
                newName: "IX_user_roles_role_id");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "user_authenticators",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_authenticators",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_authenticators",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "SrpVersion",
                table: "user_authenticators",
                newName: "srp_version");

            migrationBuilder.RenameColumn(
                name: "SrpSalt",
                table: "user_authenticators",
                newName: "srp_salt");

            migrationBuilder.RenameColumn(
                name: "SrpLogin",
                table: "user_authenticators",
                newName: "srp_login");

            migrationBuilder.RenameColumn(
                name: "SrpKeyWrapVersion",
                table: "user_authenticators",
                newName: "srp_key_wrap_version");

            migrationBuilder.RenameColumn(
                name: "SrpEncryptedVerifierWrapKey",
                table: "user_authenticators",
                newName: "srp_encrypted_verifier_wrapKey");

            migrationBuilder.RenameColumn(
                name: "SrpEncryptedVerifier",
                table: "user_authenticators",
                newName: "srp_encrypted_verifier");

            migrationBuilder.RenameColumn(
                name: "SrpAsymmetricKeyId",
                table: "user_authenticators",
                newName: "srp_asymmetric_key_id");

            migrationBuilder.RenameColumn(
                name: "LastUsedAt",
                table: "user_authenticators",
                newName: "last_used_at");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "user_authenticators",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "user_authenticators",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_UserAuthenticators_UserId",
                table: "user_authenticators",
                newName: "IX_user_authenticators_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_statuses",
                table: "statuses",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_genders",
                table: "genders",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_countries",
                table: "countries",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_authenticators",
                table: "user_authenticators",
                column: "id");

            migrationBuilder.CreateTable(
                name: "deks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    encrypted_value = table.Column<string>(type: "text", nullable: false),
                    crypto_version = table.Column<int>(type: "integer", nullable: false),
                    dek_type = table.Column<int>(type: "integer", nullable: false),
                    update_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deks", x => x.id);
                    table.ForeignKey(
                        name: "FK_deks_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recovery_keys",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    encrypted_value = table.Column<string>(type: "text", nullable: false),
                    crypto_version = table.Column<int>(type: "integer", nullable: false),
                    key_hint = table.Column<string>(type: "text", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    used_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recovery_keys", x => x.id);
                    table.ForeignKey(
                        name: "FK_recovery_keys_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_deks_UserId",
                table: "deks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_recovery_keys_UserId",
                table: "recovery_keys",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_authenticators_users_user_id",
                table: "user_authenticators",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_user_id",
                table: "user_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_authenticators_users_user_id",
                table: "user_authenticators");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_user_id",
                table: "user_roles");

            migrationBuilder.DropTable(
                name: "deks");

            migrationBuilder.DropTable(
                name: "recovery_keys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_statuses",
                table: "statuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_genders",
                table: "genders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_countries",
                table: "countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_authenticators",
                table: "user_authenticators");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "statuses",
                newName: "Statuses");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "genders",
                newName: "Genders");

            migrationBuilder.RenameTable(
                name: "countries",
                newName: "Countries");

            migrationBuilder.RenameTable(
                name: "user_roles",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "user_authenticators",
                newName: "UserAuthenticators");

            migrationBuilder.RenameColumn(
                name: "login",
                table: "Users",
                newName: "Login");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "id_status",
                table: "Users",
                newName: "IdStatus");

            migrationBuilder.RenameColumn(
                name: "id_gender",
                table: "Users",
                newName: "IdGender");

            migrationBuilder.RenameColumn(
                name: "id_country",
                table: "Users",
                newName: "IdCountry");

            migrationBuilder.RenameColumn(
                name: "date_update",
                table: "Users",
                newName: "DateUpdate");

            migrationBuilder.RenameColumn(
                name: "date_registration",
                table: "Users",
                newName: "DateRegistration");

            migrationBuilder.RenameColumn(
                name: "date_entry",
                table: "Users",
                newName: "DateEntry");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Statuses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Statuses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Roles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Roles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Genders",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Genders",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Countries",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Countries",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "UserRoles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserRoles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_role_id",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "UserAuthenticators",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserAuthenticators",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserAuthenticators",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "srp_version",
                table: "UserAuthenticators",
                newName: "SrpVersion");

            migrationBuilder.RenameColumn(
                name: "srp_salt",
                table: "UserAuthenticators",
                newName: "SrpSalt");

            migrationBuilder.RenameColumn(
                name: "srp_login",
                table: "UserAuthenticators",
                newName: "SrpLogin");

            migrationBuilder.RenameColumn(
                name: "srp_key_wrap_version",
                table: "UserAuthenticators",
                newName: "SrpKeyWrapVersion");

            migrationBuilder.RenameColumn(
                name: "srp_encrypted_verifier_wrapKey",
                table: "UserAuthenticators",
                newName: "SrpEncryptedVerifierWrapKey");

            migrationBuilder.RenameColumn(
                name: "srp_encrypted_verifier",
                table: "UserAuthenticators",
                newName: "SrpEncryptedVerifier");

            migrationBuilder.RenameColumn(
                name: "srp_asymmetric_key_id",
                table: "UserAuthenticators",
                newName: "SrpAsymmetricKeyId");

            migrationBuilder.RenameColumn(
                name: "last_used_at",
                table: "UserAuthenticators",
                newName: "LastUsedAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "UserAuthenticators",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "UserAuthenticators",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_user_authenticators_UserId",
                table: "UserAuthenticators",
                newName: "IX_UserAuthenticators_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genders",
                table: "Genders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countries",
                table: "Countries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAuthenticators",
                table: "UserAuthenticators",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserSecurityAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetType = table.Column<int>(type: "integer", nullable: false),
                    CryptoVersion = table.Column<int>(type: "integer", nullable: false),
                    EncryptedValue = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecurityAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSecurityAssets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdCountry",
                table: "Users",
                column: "IdCountry");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdGender",
                table: "Users",
                column: "IdGender");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdStatus",
                table: "Users",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurityAssets_UserId",
                table: "UserSecurityAssets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAuthenticators_Users_UserId",
                table: "UserAuthenticators",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Countries_IdCountry",
                table: "Users",
                column: "IdCountry",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Genders_IdGender",
                table: "Users",
                column: "IdGender",
                principalTable: "Genders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Statuses_IdStatus",
                table: "Users",
                column: "IdStatus",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
