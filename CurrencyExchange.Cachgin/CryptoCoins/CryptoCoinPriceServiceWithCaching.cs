using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.ConfigModels;
using Microsoft.Extensions.Caching.Memory;
using CurrencyExchange.Core.HelperFunctions;
using CurrencyExchange.Core.DTOs.CryptoCoins;

namespace CurrencyExchange.Caching.CryptoCoins
{
    public class CryptoCoinPriceServiceWithCaching
    {
        private const string CacheCryptoKey = "CryptoCoinPriceCache";
        private readonly IMemoryCache _memoryCache;
        private readonly ICommonFunctions _commonFunctions;
        public CryptoCoinPriceServiceWithCaching(IMemoryCache memoryCache, ICommonFunctions commonFunctions)
        {
            _commonFunctions = commonFunctions;
            _memoryCache = memoryCache;

            if (!_memoryCache.TryGetValue(CacheCryptoKey, out _))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1024);
                _memoryCache.Set(CacheCryptoKey, _commonFunctions.GetCryptoCoinPrices().Result, cacheEntryOptions);
            }
        }
        public List<CryptoCoinPriceDto> GetCryptoCoinPrice()
        {
            return _memoryCache.Get<List<CryptoCoinPriceDto>>(CacheCryptoKey);
        }
    }
}