using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class MenuItemMod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MenuItem",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Image",
                table: "MenuItem",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "MenuItem");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "MenuItem");
        }
    }
}
