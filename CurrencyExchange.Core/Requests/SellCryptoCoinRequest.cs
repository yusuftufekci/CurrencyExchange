using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Requests
{
    public class SellCryptoCoinRequest
    {
        public string CoinToSell { get; set; }
        public double Amount { get; set; }
    }
}
