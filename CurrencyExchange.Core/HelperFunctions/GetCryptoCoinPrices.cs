using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.HelperFunctions
{
    public class GetCryptoCoinPrices
    {
        private readonly ICryptoCoinService _cryptoCoinService;
        private readonly ICryptoCoinPriceService _cryptoCoinPriceService;

        public GetCryptoCoinPrices(ICryptoCoinService cryptoCoinService, ICryptoCoinPriceService cryptoCoinPriceService)
        {
            _cryptoCoinService = cryptoCoinService;
            _cryptoCoinPriceService = cryptoCoinPriceService;
        }

        public void dene()
        {
           
        }

    }

}
