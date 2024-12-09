using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagerPersistence.Migrations
{
    public partial class DocumentsKnowTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Metadatas_DocumentMetadataDaoId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_DocumentMetadataDaoId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DocumentMetadataDaoId",
                table: "Tags");

            migrationBuilder.CreateTable(
                name: "DocumentMetadataDaoTagDao",
                columns: table => new
                {
                    MetadatasId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentMetadataDaoTagDao", x => new { x.MetadatasId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_DocumentMetadataDaoTagDao_Metadatas_MetadatasId",
                        column: x => x.MetadatasId,
                        principalTable: "Metadatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentMetadataDaoTagDao_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentMetadataDaoTagDao_TagsId",
                table: "DocumentMetadataDaoTagDao",
                column: "TagsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentMetadataDaoTagDao");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentMetadataDaoId",
                table: "Tags",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DocumentMetadataDaoId",
                table: "Tags",
                column: "DocumentMetadataDaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Metadatas_DocumentMetadataDaoId",
                table: "Tags",
                column: "DocumentMetadataDaoId",
                principalTable: "Metadatas",
                principalColumn: "Id");
        }
    }
}
