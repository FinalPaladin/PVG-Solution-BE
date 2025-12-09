using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PVG.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductCategoryInactive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ProductCategory");

            migrationBuilder.DropColumn(
                name: "DeletedByName",
                table: "ProductCategory");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ProductCategory");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductCategory");

            migrationBuilder.AddColumn<bool>(
                name: "Inactive",
                table: "ProductCategory",
                type: "TINYINT(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inactive",
                table: "ProductCategory");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "ProductCategory",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "DeletedByName",
                table: "ProductCategory",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ProductCategory",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductCategory",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
