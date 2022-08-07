using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Requests
{
    public class BuyCoinRequest
    {
        public string CoinToBuy { get; set; }
        public string BuyWİthThisCoin { get; set; }
        public double Amount { get; set; }
    }
}
