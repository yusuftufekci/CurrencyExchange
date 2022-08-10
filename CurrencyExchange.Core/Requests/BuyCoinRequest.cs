namespace CurrencyExchange.Core.Requests
{
    public class BuyCoinRequest
    {
        public string CoinToBuy { get; set; }
        public string BuyWIthThisCoin { get; set; }
        public double Amount { get; set; }
    }
}
