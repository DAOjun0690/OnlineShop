using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Migrations
{
    public partial class FixOrderReceiverColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiverAdress",
                table: "Order",
                newName: "ReceiverSecondAddress");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Order",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverFirstAddress",
                table: "Order",
                type: "TEXT",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SelectedDeliveryAddress",
                table: "Order",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SelectedDeliveryMethod",
                table: "Order",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ReceiverFirstAddress",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "SelectedDeliveryAddress",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "SelectedDeliveryMethod",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "ReceiverSecondAddress",
                table: "Order",
                newName: "ReceiverAdress");
        }
    }
}
