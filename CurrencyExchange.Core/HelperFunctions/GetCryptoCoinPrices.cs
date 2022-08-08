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


        public static async Task<List<CryptoCoinPriceDto>> AsyncGetCryptoCoinPrices()
        {
            List<CryptoCoinPriceDto> cryptoCoinPrices = new List<CryptoCoinPriceDto>();

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(1);
                HttpResponseMessage response = await client.GetAsync("https://api.binance.com/api/v3/ticker/price");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var ResponceString = await response.Content.ReadAsStringAsync();


                    var ResponceObject = JsonConvert.DeserializeObject<List<CryptoCoinPriceDto>>(ResponceString);

                    foreach (var item in ResponceObject)
                    {
                        var coinPrice = new CryptoCoinPriceDto
                        {
                            price = item.price,
                            symbol = item.symbol
                         };
                       
                        cryptoCoinPrices.Add(coinPrice);
                    }

                    return cryptoCoinPrices;


                }

                return null;
            }
        }
    }
}

