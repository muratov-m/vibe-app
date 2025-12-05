using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VibeApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Telegram = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LinkedIn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Photo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    HasStartup = table.Column<bool>(type: "boolean", nullable: false),
                    StartupStage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartupDescription = table.Column<string>(type: "text", nullable: false),
                    StartupName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CanHelp = table.Column<string>(type: "text", nullable: false),
                    NeedsHelp = table.Column<string>(type: "text", nullable: false),
                    AiUsage = table.Column<string>(type: "text", nullable: false),
                    Custom1 = table.Column<string>(type: "text", nullable: false),
                    Custom2 = table.Column<string>(type: "text", nullable: false),
                    Custom3 = table.Column<string>(type: "text", nullable: false),
                    Custom4 = table.Column<string>(type: "text", nullable: false),
                    Custom5 = table.Column<string>(type: "text", nullable: false),
                    Custom6 = table.Column<string>(type: "text", nullable: false),
                    Custom7 = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "UserLookingFors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false),
                    LookingFor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLookingFors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLookingFors_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false),
                    Skill = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSkills_UserProfiles_UserProfileId",
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

            migrationBuilder.CreateIndex(
                name: "IX_UserLookingFors_UserProfileId",
                table: "UserLookingFors",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkills_UserProfileId",
                table: "UserSkills",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropTable(
                name: "UserLookingFors");

            migrationBuilder.DropTable(
                name: "UserSkills");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
