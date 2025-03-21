using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ActiveX_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedAddressNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "Address");
        }
    }
}
