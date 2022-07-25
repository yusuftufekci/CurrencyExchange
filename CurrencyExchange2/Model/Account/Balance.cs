using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Model.Account
{
    public class Balance
    {
        [Key]
        public int BalanceId { get; set; }
        public int CoinId { get; set; }
        public string CoinName { get; set; }

        [RegularExpression(@"^\d+(.\d{1,2})?$")]
        public double TotalBalance { get; set; } = 0;
        public Account Account { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}
