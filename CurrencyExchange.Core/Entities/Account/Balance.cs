using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.Core.Entities.CryptoCoins;

namespace CurrencyExchange.Core.Entities.Account
{
    public class Balance : BaseEntity
    {
        public double TotalBalance { get; set; } = 0;

        public CryptoCoin CryptoCoin { get; set; }
        public int CryptoCoinId { get; set; }
        public Account Account { get; set; }
        public int AccountId { get; set; }


    }
}
