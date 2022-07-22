using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Model.Account
{
    public class Balance
    {
        [Key]
        public int UserId { get; set; }
        public double BTC { get; set; } = 0;
        public double ETH { get; set; } = 0;
        public double ARPA { get; set; } = 0;
        public double ICP { get; set; } = 0;
        public double DOGE { get; set; } = 0;
        public double USDT { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}
