using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class ExternalTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RealEstatePropertyPhotoId",
                table: "RealEstatePropertyPhotos",
                newName: "RealEstatePropertyId");

            migrationBuilder.AddColumn<int>(
                name: "AgentId",
                table: "RealEstatePropertys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "RealEstatePropertys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Buyer",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Seller",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatePropertys_AgentId",
                table: "RealEstatePropertys",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatePropertys_CustomerId",
                table: "RealEstatePropertys",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatePropertyPhotos_RealEstatePropertyId",
                table: "RealEstatePropertyPhotos",
                column: "RealEstatePropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstatePropertyPhotos_RealEstatePropertys_RealEstatePropertyId",
                table: "RealEstatePropertyPhotos",
                column: "RealEstatePropertyId",
                principalTable: "RealEstatePropertys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstatePropertys_Agents_AgentId",
                table: "RealEstatePropertys",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstatePropertys_Customers_CustomerId",
                table: "RealEstatePropertys",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstatePropertyPhotos_RealEstatePropertys_RealEstatePropertyId",
                table: "RealEstatePropertyPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_RealEstatePropertys_Agents_AgentId",
                table: "RealEstatePropertys");

            migrationBuilder.DropForeignKey(
                name: "FK_RealEstatePropertys_Customers_CustomerId",
                table: "RealEstatePropertys");

            migrationBuilder.DropIndex(
                name: "IX_RealEstatePropertys_AgentId",
                table: "RealEstatePropertys");

            migrationBuilder.DropIndex(
                name: "IX_RealEstatePropertys_CustomerId",
                table: "RealEstatePropertys");

            migrationBuilder.DropIndex(
                name: "IX_RealEstatePropertyPhotos_RealEstatePropertyId",
                table: "RealEstatePropertyPhotos");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "RealEstatePropertys");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "RealEstatePropertys");

            migrationBuilder.DropColumn(
                name: "Buyer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Seller",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "RealEstatePropertyId",
                table: "RealEstatePropertyPhotos",
                newName: "RealEstatePropertyPhotoId");
        }
    }
}
