namespace CurrencyExchange.Core.Entities.CryptoCoins
{
    public class CryptoCoinPrice : BaseEntity
    {
        public string Symbol { get; set; }
        public string Price { get; set; }
    }
}
