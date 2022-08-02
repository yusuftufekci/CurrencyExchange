﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Requests
{
    public class UserRegisterRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


    }
}
