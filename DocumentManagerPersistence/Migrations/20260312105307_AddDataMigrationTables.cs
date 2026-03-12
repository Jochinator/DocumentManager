using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagerPersistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDataMigrationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataMigrations",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Completed = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataMigrations", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "DataMigrationErrors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MigrationName = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: false),
                    DataMigrationDaoName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataMigrationErrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataMigrationErrors_DataMigrations_DataMigrationDaoName",
                        column: x => x.DataMigrationDaoName,
                        principalTable: "DataMigrations",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataMigrationErrors_DataMigrationDaoName",
                table: "DataMigrationErrors",
                column: "DataMigrationDaoName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataMigrationErrors");

            migrationBuilder.DropTable(
                name: "DataMigrations");
        }
    }
}
