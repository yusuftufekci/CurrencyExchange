using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Entities.Account
{
    public class UserBalanceHistory : BaseEntity
    {
        public Account Account { get; set; }
        public int AccountId { get; set; }
        public string MessageForChanging { get; set; }

        public string BoughtCryptoCoin { get; set; }

        public string SoldCryptoCoin { get; set; }


        public double ChangedAmount { get; set; }
    }
}
