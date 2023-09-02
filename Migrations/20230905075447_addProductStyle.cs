using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Migrations
{
    public partial class addProductStyle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductStyleId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Promotion",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProductStyle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStyle", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductStyle_ProductStyleId",
                table: "Product");

            migrationBuilder.DropTable(
                name: "ProductStyle");

            migrationBuilder.DropIndex(
                name: "IX_Product_ProductStyleId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ProductStyleId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Promotion",
                table: "Product");
        }
    }
}
