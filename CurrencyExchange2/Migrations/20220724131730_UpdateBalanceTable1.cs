using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchange2.Migrations
{
    public partial class UpdateBalanceTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoinName",
                table: "Balances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoinName",
                table: "Balances");
        }
    }
}
