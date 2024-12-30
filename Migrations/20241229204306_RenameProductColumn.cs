using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ActiveX_Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameProductColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "stars",
                table: "Reviews",
                newName: "Stars");

            migrationBuilder.RenameColumn(
                name: "ModelFile",
                table: "Products",
                newName: "File3DModel");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(12,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stars",
                table: "Reviews",
                newName: "stars");

            migrationBuilder.RenameColumn(
                name: "File3DModel",
                table: "Products",
                newName: "ModelFile");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)",
                oldNullable: true);
        }
    }
}
