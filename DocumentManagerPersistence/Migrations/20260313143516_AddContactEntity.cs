using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagerPersistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContactEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContactId",
                table: "Metadatas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Metadatas_ContactId",
                table: "Metadatas",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Name",
                table: "Contacts",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Metadatas_Contacts_ContactId",
                table: "Metadatas",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Metadatas_Contacts_ContactId",
                table: "Metadatas");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Metadatas_ContactId",
                table: "Metadatas");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Metadatas");
        }
    }
}
