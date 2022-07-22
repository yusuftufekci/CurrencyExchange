using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Model.Crypto
{
    public class CryptoCoin
    {
        [Key]
        public int CoinId { get; set; }
        public string symbol { get; set; }
        public string price { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

    }
}
