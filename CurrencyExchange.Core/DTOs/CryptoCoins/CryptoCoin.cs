using CurrencyExchange.Core.Entities.Account;

namespace CurrencyExchange.Core.Entities.CryptoCoins
{
    public class CryptoCoin
    {
        public string CoinName { get; set; }
        public List<Balance> Balances { get; set; }

    }
}
