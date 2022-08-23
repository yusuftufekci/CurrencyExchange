using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.CryptoCoins;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CurrencyExchange.Core.HelperFunctions;

// Şu an da kullanılmıyor

namespace CurrencyExchange.Cachgin
{
    public class CryptoCoinPriceServiceWithCaching 
    {
        private const string CacheCryptoKey = "CryptoCoinPriceCache";
        private readonly IMemoryCache _memoryCache;


        public CryptoCoinPriceServiceWithCaching(IMemoryCache memoryCache )
        {
            _memoryCache = memoryCache;
            
            if (!_memoryCache.TryGetValue(CacheCryptoKey, out _))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1024);
                _memoryCache.Set(CacheCryptoKey, GetCryptoCoinPrices.AsyncGetCryptoCoinPrices().Result, cacheEntryOptions);
            }
        }

  

        public Task<IEnumerable<CryptoCoin>> GetAllAsync()
        {
            return Task.FromResult(_memoryCache.Get<IEnumerable<CryptoCoin>>(CacheCryptoKey));
        }

        public List<CryptoCoinPriceDto> GetCryptoCoinPrice()
        {
            return _memoryCache.Get<List<CryptoCoinPriceDto>>(CacheCryptoKey);
        }

        public Task<CustomResponseDto<List<CryptoCoin>>> GetCryptoCoin()
        {
            var cryptoCoin = _memoryCache.Get<List<CryptoCoin>>(CacheCryptoKey);

            return Task.FromResult(CustomResponseDto<List<CryptoCoin>>.Succes(200, cryptoCoin));
        }

        public IQueryable<CryptoCoin> Where(Expression<Func<CryptoCoin, bool>> expression)
        {
            return _memoryCache.Get<List<CryptoCoin>>(CacheCryptoKey).Where(expression.Compile()).AsQueryable();
        }
    }
}