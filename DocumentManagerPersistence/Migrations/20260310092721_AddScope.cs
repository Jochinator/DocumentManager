using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagerPersistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "Metadatas",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Metadatas");
        }
    }
}
