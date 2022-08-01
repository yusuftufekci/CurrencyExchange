using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.DTOs
{
    public class UserInformationDto
    {
        public string UserEmail { get; set; }
        public string UserAccountName { get; set; }
        public List<BalanceDto> UserBalances { get; set; }
    }
}
