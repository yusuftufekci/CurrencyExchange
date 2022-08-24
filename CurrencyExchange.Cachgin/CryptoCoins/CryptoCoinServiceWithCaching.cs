using CurrencyExchange.Core.Entities.CryptoCoins;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyExchange.Caching.CryptoCoins
{
    public class CryptoCoinServiceWithCaching
    {
        private const string CacheCryptoKey = "CryptoCoinCache";
        private readonly IMemoryCache _memoryCache;
        public CryptoCoinServiceWithCaching(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;

            if (!_memoryCache.TryGetValue(CacheCryptoKey, out _))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1024);
                _memoryCache.Set(CacheCryptoKey, Core.HelperFunctions.GetCryptoCoins.AsyncGetCryptoCoins().Result, cacheEntryOptions);
            }
        }
        public List<CryptoCoin> GetCryptoCoins()
        {
            return _memoryCache.Get<List<CryptoCoin>>(CacheCryptoKey);
        }
    }
}
