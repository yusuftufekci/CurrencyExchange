using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.DTOs
{
    public class UserTransactionHistoryDto
    {
        public string AccountName { get; set; }

        public string MessageForChanging { get; set; }

        public string ExchangedCoinName { get; set; }

        public double ChangedAmount { get; set; }
    }
}
