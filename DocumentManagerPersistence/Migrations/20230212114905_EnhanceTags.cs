using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagerPersistence.Migrations
{
    public partial class EnhanceTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "value",
                table: "Tags",
                newName: "Value");

            migrationBuilder.AddColumn<bool>(
                name: "IsManualOnly",
                table: "Tags",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsManualOnly",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Tags",
                newName: "value");
        }
    }
}
