using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VibeApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddParsedFieldsToUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove old City and Country fields (data will be lost)
            migrationBuilder.DropColumn(
                name: "City",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "UserProfiles");

            // Add new Parsed* fields (will be filled by AI processing)
            migrationBuilder.AddColumn<string>(
                name: "ParsedCity",
                table: "UserProfiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParsedCountry",
                table: "UserProfiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParsedInterests",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParsedMainActivity",
                table: "UserProfiles",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParsedShortBio",
                table: "UserProfiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove Parsed* fields
            migrationBuilder.DropColumn(
                name: "ParsedCity",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ParsedCountry",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ParsedInterests",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ParsedMainActivity",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ParsedShortBio",
                table: "UserProfiles");

            // Restore old City and Country fields
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "UserProfiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "UserProfiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}

