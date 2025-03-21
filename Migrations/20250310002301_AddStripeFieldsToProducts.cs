using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ActiveX_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeFieldsToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripePriceId",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeProductId",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripePriceId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StripeProductId",
                table: "Products");
        }
    }
}
