using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchange.Repository.Migrations
{
    public partial class added_update_message_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "ResponseMessages",
                newName: "LogValue");

            migrationBuilder.AddColumn<string>(
                name: "ApiValue",
                table: "ResponseMessages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiValue",
                table: "ResponseMessages");

            migrationBuilder.RenameColumn(
                name: "LogValue",
                table: "ResponseMessages",
                newName: "Value");
        }
    }
}
