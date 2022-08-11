using CurrencyExchange.Core.Entities.CryptoCoins;

namespace CurrencyExchange.Core.Entities.Account
{
    public class Balance : BaseEntity
    {
        public double TotalBalance { get; set; } = 0;
        public string CryptoCoinName { get; set; }
        public Account Account { get; set; }
        public int AccountId { get; set; }


    }
}