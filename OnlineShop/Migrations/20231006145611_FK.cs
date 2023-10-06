using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Migrations
{
    public partial class FK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManufacturingCustomDate",
                table: "ProductHistory",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManufacturingMethod",
                table: "ProductHistory",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManufacturingTime",
                table: "ProductHistory",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "OrderItem",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManufacturingCustomDate",
                table: "ProductHistory");

            migrationBuilder.DropColumn(
                name: "ManufacturingMethod",
                table: "ProductHistory");

            migrationBuilder.DropColumn(
                name: "ManufacturingTime",
                table: "ProductHistory");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "OrderItem");
        }
    }
}
