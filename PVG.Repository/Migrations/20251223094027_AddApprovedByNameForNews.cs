using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PVG.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovedByNameForNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedByName",
                table: "News",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedByName",
                table: "News");
        }
    }
}
