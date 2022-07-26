using CurrencyExchange2.Model.Authentication;
using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Model.Account
{
    public class UserBalanceHistory
    {
        [Key]
        public int BalanceHistoryId { get; set; }

        public Account Account { get; set; }

        public string MessageForChanging { get; set; }

        public string ExchangedCoinName { get; set; }

        public double ChangedAmount { get; set; }


    }
}
