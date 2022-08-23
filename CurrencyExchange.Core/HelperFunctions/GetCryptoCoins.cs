using CurrencyExchange.Core.Entities.CryptoCoins;
using Newtonsoft.Json.Linq;

namespace CurrencyExchange.Core.HelperFunctions
{
    public class GetCryptoCoins
    {
        public static async Task<List<CryptoCoin>?> AsyncGetCryptoCoins()
        {
            var cryptoCoins = new List<CryptoCoin>();
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(1);
            var response =
                await client.GetAsync(
                    "https://api.nomics.com/v1/currencies/ticker?key=ab9543f3c307afc219e1b55e9527559447536691");
            if (response.StatusCode != System.Net.HttpStatusCode.OK) return null;
            var responceString = await response.Content.ReadAsStringAsync();
            var root = (JContainer)JToken.Parse(responceString);
            var list = root.DescendantsAndSelf()
                .OfType<JProperty>()
                .Where(p => p.Name == "id")
                .Select(p => p.Value.Value<string>())
                .ToList();
            cryptoCoins.AddRange(list.Select(item => new CryptoCoin { CoinName = item }));

            return cryptoCoins;
        }
    }
}