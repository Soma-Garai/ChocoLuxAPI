using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChocoLuxAPI.Migrations
{
    /// <inheritdoc />
    public partial class MakingExpiresAtNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
             name: "CreatedAt",
             table: "TblSession",
             nullable: true,
             oldClrType: typeof(DateTime),
             oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
             name: "ExpiresAt",
             table: "TblSession",
             nullable: true,
             oldClrType: typeof(DateTime),
             oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            table: "TblSession",
            type: "datetime2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
            name: "ExpiresAt",
            table: "TblSession",
            type: "datetime2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldNullable: true);
        }
    }
}
