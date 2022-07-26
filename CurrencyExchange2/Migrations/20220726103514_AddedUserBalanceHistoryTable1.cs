using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchange2.Migrations
{
    public partial class AddedUserBalanceHistoryTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBalanceHistories",
                columns: table => new
                {
                    BalanceHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    MessageForChanging = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangedCoinName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAmount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBalanceHistories", x => x.BalanceHistoryId);
                    table.ForeignKey(
                        name: "FK_UserBalanceHistories_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBalanceHistories_AccountId",
                table: "UserBalanceHistories",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBalanceHistories");
        }
    }
}
