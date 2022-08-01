using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Requests
{
    public class DepositFundRequest
    {
        public string? UserEmail { get; set; }

        public double TotalBalance { get; set; } = 0;
    }
}
