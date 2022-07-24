using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Model.Crypto
{
    public class CoinType
    {
        [Key]
        public int CoinId { get; set; }
        public string CoinName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

    }
}
