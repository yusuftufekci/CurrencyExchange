using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Model.Account
{
    public class UserBalancesInfo
    {
        public string CoinName { get; set; }

        [RegularExpression(@"^\d+(.\d{1,2})?$")]
        public double TotalBalance { get; set; } = 0;
    }
}
