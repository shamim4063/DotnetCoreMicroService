using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UserManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "user_management");

            migrationBuilder.CreateTable(
                name: "app_role",
                schema: "user_management",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsCommissionRole = table.Column<bool>(type: "boolean", nullable: false),
                    CommissionItemId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "app_user",
                schema: "user_management",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    PasswordCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PasswordLastChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MfaEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "menu",
                schema: "user_management",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Route = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_menu_menu_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "user_management",
                        principalTable: "menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "permitted_action",
                schema: "user_management",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KeyEnum = table.Column<int>(type: "integer", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permitted_action", x => x.Id);
                    table.UniqueConstraint("AK_permitted_action_KeyEnum", x => x.KeyEnum);
                });

            migrationBuilder.CreateTable(
                name: "user_warehouse_projection",
                schema: "user_management",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    WarehouseName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    OtherMetadata = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_warehouse_projection", x => new { x.UserId, x.WarehouseId });
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                schema: "user_management",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_role", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_user_role_app_role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "user_management",
                        principalTable: "app_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_role_app_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "user_management",
                        principalTable: "app_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "menu_action",
                schema: "user_management",
                columns: table => new
                {
                    ActionsId = table.Column<long>(type: "bigint", nullable: false),
                    MenuId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_action", x => new { x.ActionsId, x.MenuId });
                    table.ForeignKey(
                        name: "FK_menu_action_menu_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "user_management",
                        principalTable: "menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_menu_action_permitted_action_ActionsId",
                        column: x => x.ActionsId,
                        principalSchema: "user_management",
                        principalTable: "permitted_action",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "permission",
                schema: "user_management",
                columns: table => new
                {
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    MenuId = table.Column<long>(type: "bigint", nullable: false),
                    ActionKeyEnum = table.Column<int>(type: "integer", nullable: false),
                    Allowed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission", x => new { x.RoleId, x.MenuId, x.ActionKeyEnum });
                    table.ForeignKey(
                        name: "FK_permission_app_role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "user_management",
                        principalTable: "app_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_permission_menu_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "user_management",
                        principalTable: "menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_permission_permitted_action_ActionKeyEnum",
                        column: x => x.ActionKeyEnum,
                        principalSchema: "user_management",
                        principalTable: "permitted_action",
                        principalColumn: "KeyEnum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_app_role_Name",
                schema: "user_management",
                table: "app_role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_app_user_Email",
                schema: "user_management",
                table: "app_user",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_Key",
                schema: "user_management",
                table: "menu",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_ParentId",
                schema: "user_management",
                table: "menu",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_menu_action_MenuId",
                schema: "user_management",
                table: "menu_action",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_permission_ActionKeyEnum",
                schema: "user_management",
                table: "permission",
                column: "ActionKeyEnum");

            migrationBuilder.CreateIndex(
                name: "IX_permission_MenuId",
                schema: "user_management",
                table: "permission",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_permitted_action_KeyEnum",
                schema: "user_management",
                table: "permitted_action",
                column: "KeyEnum",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_role_RoleId",
                schema: "user_management",
                table: "user_role",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "menu_action",
                schema: "user_management");

            migrationBuilder.DropTable(
                name: "permission",
                schema: "user_management");

            migrationBuilder.DropTable(
                name: "user_role",
                schema: "user_management");

            migrationBuilder.DropTable(
                name: "user_warehouse_projection",
                schema: "user_management");

            migrationBuilder.DropTable(
                name: "menu",
                schema: "user_management");

            migrationBuilder.DropTable(
                name: "permitted_action",
                schema: "user_management");

            migrationBuilder.DropTable(
                name: "app_role",
                schema: "user_management");

            migrationBuilder.DropTable(
                name: "app_user",
                schema: "user_management");
        }
    }
}
