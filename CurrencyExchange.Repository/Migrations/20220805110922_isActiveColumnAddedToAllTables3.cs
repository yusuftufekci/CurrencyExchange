using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchange.Repository.Migrations
{
    public partial class isActiveColumnAddedToAllTables3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBalanceHistories_Accounts_AccountId",
                table: "UserBalanceHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBalanceHistories",
                table: "UserBalanceHistories");

            migrationBuilder.RenameTable(
                name: "UserBalanceHistories",
                newName: "TransactionHistory");

            migrationBuilder.RenameColumn(
                name: "MessageForChanging",
                table: "TransactionHistory",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ChangedAmount",
                table: "TransactionHistory",
                newName: "ChangedAmountBoughtCryptoCoin");

            migrationBuilder.RenameIndex(
                name: "IX_UserBalanceHistories_AccountId",
                table: "TransactionHistory",
                newName: "IX_TransactionHistory_AccountId");

            migrationBuilder.AddColumn<decimal>(
                name: "ChangedAmountSoldCryptoCoin",
                table: "TransactionHistory",
                type: "decimal(15,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionHistory",
                table: "TransactionHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistory_Accounts_AccountId",
                table: "TransactionHistory",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistory_Accounts_AccountId",
                table: "TransactionHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionHistory",
                table: "TransactionHistory");

            migrationBuilder.DropColumn(
                name: "ChangedAmountSoldCryptoCoin",
                table: "TransactionHistory");

            migrationBuilder.RenameTable(
                name: "TransactionHistory",
                newName: "UserBalanceHistories");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "UserBalanceHistories",
                newName: "MessageForChanging");

            migrationBuilder.RenameColumn(
                name: "ChangedAmountBoughtCryptoCoin",
                table: "UserBalanceHistories",
                newName: "ChangedAmount");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionHistory_AccountId",
                table: "UserBalanceHistories",
                newName: "IX_UserBalanceHistories_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBalanceHistories",
                table: "UserBalanceHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBalanceHistories_Accounts_AccountId",
                table: "UserBalanceHistories",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
