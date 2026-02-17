using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecificDocumentationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpecificDocumentations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentsTabId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificDocumentations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecificDocumentations");
        }
    }
}
