using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VibeApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCustomFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCustomArray1s");

            migrationBuilder.DropTable(
                name: "UserCustomArray2s");

            migrationBuilder.DropTable(
                name: "UserCustomArray3s");

            migrationBuilder.DropTable(
                name: "UserCustomArray4s");

            migrationBuilder.DropTable(
                name: "UserCustomArray5s");

            migrationBuilder.DropTable(
                name: "UserCustomArray6s");

            migrationBuilder.DropTable(
                name: "UserCustomArray7s");

            migrationBuilder.DropColumn(
                name: "Custom1",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Custom2",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Custom3",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Custom4",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Custom5",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Custom6",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Custom7",
                table: "UserProfiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Custom1",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Custom2",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Custom3",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Custom4",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Custom5",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Custom6",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Custom7",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserCustomArray1s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCustomArray1s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCustomArray1s_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCustomArray2s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCustomArray2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCustomArray2s_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCustomArray3s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCustomArray3s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCustomArray3s_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCustomArray4s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCustomArray4s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCustomArray4s_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCustomArray5s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCustomArray5s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCustomArray5s_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCustomArray6s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCustomArray6s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCustomArray6s_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCustomArray7s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCustomArray7s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCustomArray7s_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCustomArray1s_UserProfileId",
                table: "UserCustomArray1s",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCustomArray2s_UserProfileId",
                table: "UserCustomArray2s",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCustomArray3s_UserProfileId",
                table: "UserCustomArray3s",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCustomArray4s_UserProfileId",
                table: "UserCustomArray4s",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCustomArray5s_UserProfileId",
                table: "UserCustomArray5s",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCustomArray6s_UserProfileId",
                table: "UserCustomArray6s",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCustomArray7s_UserProfileId",
                table: "UserCustomArray7s",
                column: "UserProfileId");
        }
    }
}
