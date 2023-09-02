using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Migrations
{
    public partial class fixProductStyle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductStyle_ProductStyleId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_ProductStyleId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ProductStyleId",
                table: "Product");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStyle_ProductId",
                table: "ProductStyle",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStyle_Product_ProductId",
                table: "ProductStyle",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductStyle_Product_ProductId",
                table: "ProductStyle");

            migrationBuilder.DropIndex(
                name: "IX_ProductStyle_ProductId",
                table: "ProductStyle");

            migrationBuilder.AddColumn<int>(
                name: "ProductStyleId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductStyleId",
                table: "Product",
                column: "ProductStyleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ProductStyle_ProductStyleId",
                table: "Product",
                column: "ProductStyleId",
                principalTable: "ProductStyle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
