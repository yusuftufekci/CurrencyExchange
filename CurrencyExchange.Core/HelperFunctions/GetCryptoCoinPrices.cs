using CurrencyExchange.Core.DTOs;
using Newtonsoft.Json;

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
                    var responceString = await response.Content.ReadAsStringAsync();


                    var responceObject = JsonConvert.DeserializeObject<List<CryptoCoinPriceDto>>(responceString);

                    foreach (var item in responceObject)
                    {
                        var coinPrice = new CryptoCoinPriceDto
                        {
                            Price = item.Price,
                            Symbol = item.Symbol
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

