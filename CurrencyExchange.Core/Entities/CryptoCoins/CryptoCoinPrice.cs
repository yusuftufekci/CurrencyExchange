using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Entities.CryptoCoins
{
    public class CryptoCoinPrice : BaseEntity
    {
        public string Symbol { get; set; }
        public string Price { get; set; }
    }
}
