using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChocoLuxAPI.Migrations
{
    /// <inheritdoc />
    public partial class changedPropertyNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "product_price",
                table: "tblProducts",
                newName: "ProductPrice");

            migrationBuilder.RenameColumn(
                name: "product_name",
                table: "tblProducts",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "product_description",
                table: "tblProducts",
                newName: "ProductImagePath");

            migrationBuilder.RenameColumn(
                name: "product_ImagePath",
                table: "tblProducts",
                newName: "ProductDescription");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "tblProducts",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "product_price",
                table: "tblOrderDetails",
                newName: "ProductPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductPrice",
                table: "tblProducts",
                newName: "product_price");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "tblProducts",
                newName: "product_name");

            migrationBuilder.RenameColumn(
                name: "ProductImagePath",
                table: "tblProducts",
                newName: "product_description");

            migrationBuilder.RenameColumn(
                name: "ProductDescription",
                table: "tblProducts",
                newName: "product_ImagePath");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "tblProducts",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "ProductPrice",
                table: "tblOrderDetails",
                newName: "product_price");
        }
    }
}
