using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Entities.Authentication
{
    public class UserToken : BaseEntity
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public string? Token { get; set; }

        public DateTime ExpDate { get; set; }
    }
}
