using CurrencyExchange.Core.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Entities.Account
{
    public class Account : BaseEntity
    {
        public string AccountName { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public ICollection<Balance> Balances { get; set; }

    }
}
