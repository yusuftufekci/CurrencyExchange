using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchange2.Migrations
{
    public partial class UpdateCryptoCoins1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CryptoCoins",
                table: "CryptoCoins");

            migrationBuilder.RenameTable(
                name: "CryptoCoins",
                newName: "CryptoCoinPrices");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CryptoCoinPrices",
                table: "CryptoCoinPrices",
                column: "CoinPriceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CryptoCoinPrices",
                table: "CryptoCoinPrices");

            migrationBuilder.RenameTable(
                name: "CryptoCoinPrices",
                newName: "CryptoCoins");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CryptoCoins",
                table: "CryptoCoins",
                column: "CoinPriceId");
        }
    }
}
