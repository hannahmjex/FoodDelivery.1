using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class MenuItemFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "MenuItem",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<int>(
				name: "Image",
				table: "MenuItem",
				type: "int",
				nullable: false,
				oldClrType: typeof(string),
				oldNullable: true);
		}
	}
}
