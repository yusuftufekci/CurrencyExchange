namespace CurrencyExchange.Core.Requests
{
    public class SellCryptoCoinRequest
    {
        public string CoinToSell { get; set; }
        public double Amount { get; set; }
    }
}
