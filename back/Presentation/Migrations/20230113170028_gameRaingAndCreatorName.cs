using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presentation.Migrations
{
    /// <inheritdoc />
    public partial class gameRaingAndCreatorName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "Games",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxRating",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "MaxRating",
                table: "Games");
        }
    }
}
