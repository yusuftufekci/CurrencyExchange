﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Requests
{
    public class CreateAccountRequest
    {
        public string AccountName { get; set; }
    }
}
