using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChocoLuxAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedPaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
        name: "TblPayment",
        columns: table => new
        {
            PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
            PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_TblPayment", x => x.PaymentId);
            table.ForeignKey(
                name: "FK_TblPayment_tblOrders_OrderId",
                column: x => x.OrderId,
                principalTable: "tblOrders",
                principalColumn: "OrderId");
        });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblPayment");
        }
    }
}
