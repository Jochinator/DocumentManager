using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagerPersistence.Migrations
{
    /// <inheritdoc />
    public partial class removeSenderName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "Metadatas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "Metadatas",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
