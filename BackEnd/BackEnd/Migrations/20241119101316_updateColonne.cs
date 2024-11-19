using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class updateColonne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Svailability",
                table: "RealEstatePropertys");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "RealEstatePropertys",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "RealEstatePropertys",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Availability",
                table: "RealEstatePropertys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exposure",
                table: "RealEstatePropertys",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Availability",
                table: "RealEstatePropertys");

            migrationBuilder.DropColumn(
                name: "Exposure",
                table: "RealEstatePropertys");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "RealEstatePropertys",
                newName: "description");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "RealEstatePropertys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Svailability",
                table: "RealEstatePropertys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
