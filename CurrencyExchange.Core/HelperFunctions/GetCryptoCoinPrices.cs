using CurrencyExchange.Core.DTOs;
using Newtonsoft.Json;

namespace CurrencyExchange.Core.HelperFunctions
{
    public class GetCryptoCoinPrices
    {
        public static async Task<List<CryptoCoinPriceDto>?> AsyncGetCryptoCoinPrices()
        {
            var cryptoCoinPrices = new List<CryptoCoinPriceDto>();
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(1);
            var response = await client.GetAsync("https://api.binance.com/api/v3/ticker/price");
            if (response.StatusCode != System.Net.HttpStatusCode.OK) return null;
            var responceString = await response.Content.ReadAsStringAsync();
            var responceObject = JsonConvert.DeserializeObject<List<CryptoCoinPriceDto>>(responceString);
            cryptoCoinPrices.AddRange(responceObject.Select(item => new CryptoCoinPriceDto { Price = item.Price, Symbol = item.Symbol }));
            return cryptoCoinPrices;
        }
    }
}