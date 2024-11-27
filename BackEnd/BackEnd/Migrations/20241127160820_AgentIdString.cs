using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AgentIdString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstatePropertys_Agents_AgentId",
                table: "RealEstatePropertys");

            migrationBuilder.DropIndex(
                name: "IX_RealEstatePropertys_AgentId",
                table: "RealEstatePropertys");

            migrationBuilder.AlterColumn<string>(
                name: "AgentId",
                table: "RealEstatePropertys",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AgentsId",
                table: "RealEstatePropertys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatePropertys_AgentsId",
                table: "RealEstatePropertys",
                column: "AgentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstatePropertys_Agents_AgentsId",
                table: "RealEstatePropertys",
                column: "AgentsId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstatePropertys_Agents_AgentsId",
                table: "RealEstatePropertys");

            migrationBuilder.DropIndex(
                name: "IX_RealEstatePropertys_AgentsId",
                table: "RealEstatePropertys");

            migrationBuilder.DropColumn(
                name: "AgentsId",
                table: "RealEstatePropertys");

            migrationBuilder.AlterColumn<int>(
                name: "AgentId",
                table: "RealEstatePropertys",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstatePropertys_AgentId",
                table: "RealEstatePropertys",
                column: "AgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstatePropertys_Agents_AgentId",
                table: "RealEstatePropertys",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
