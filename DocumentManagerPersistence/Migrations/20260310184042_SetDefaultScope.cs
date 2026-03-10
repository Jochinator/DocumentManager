using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagerPersistence.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Metadatas",
                keyColumn: "Scope",
                keyValue: "",
                column: "Scope",
                value: "default");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
