using CurrencyExchange.Core.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Entities.CryptoCoins
{
    public class CryptoCoin : BaseEntity
    {
        public string CoinName { get; set; }
        public List<Balance> Balances { get; set; }

    }
}
