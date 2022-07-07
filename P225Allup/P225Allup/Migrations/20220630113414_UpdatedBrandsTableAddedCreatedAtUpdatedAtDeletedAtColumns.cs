using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace P225Allup.Migrations
{
    public partial class UpdatedBrandsTableAddedCreatedAtUpdatedAtDeletedAtColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Brands",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Brands",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Brands",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Brands");
        }
    }
}
