using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchange2.Migrations
{
    public partial class IpAdressFieldAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAdress",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAdress",
                table: "Users");
        }
    }
}
