using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagerPersistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RuleId",
                table: "Tags",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RuleId",
                table: "Contacts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RuleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RuleType = table.Column<int>(type: "INTEGER", nullable: false),
                    PredicateValue = table.Column<string>(type: "TEXT", nullable: true),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rules_Rules_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Rules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_RuleId",
                table: "Tags",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_RuleId",
                table: "Contacts",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_ParentId",
                table: "Rules",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Rules_RuleId",
                table: "Contacts",
                column: "RuleId",
                principalTable: "Rules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Rules_RuleId",
                table: "Tags",
                column: "RuleId",
                principalTable: "Rules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Rules_RuleId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Rules_RuleId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropIndex(
                name: "IX_Tags_RuleId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_RuleId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "RuleId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "RuleId",
                table: "Contacts");
        }
    }
}
