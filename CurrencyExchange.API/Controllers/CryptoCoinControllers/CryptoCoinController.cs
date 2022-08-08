using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.CryptoCoinControllers
{
    public class CryptoCoinController : CustomBaseController
    {
        private readonly ICryptoCoinService _cryptoCoinService;
        private readonly ICryptoCoinPriceService _cryptoCoinPriceService;

        public CryptoCoinController(ICryptoCoinService cryptoCoinService, ICryptoCoinPriceService cryptoCoinPriceService )
        {
            _cryptoCoinService = cryptoCoinService;
            _cryptoCoinPriceService = cryptoCoinPriceService;
        }
        [HttpPost("GetCryptoCoinList")]
        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        public async Task<IActionResult> GetCryptoCoinList()
        {
            return CreateActionResult(await _cryptoCoinService.CryptoCoin());
        }
        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("GetCryptoCoinPriceList")]
        public async Task<IActionResult> GetCryptoCoinPriceList()
        {
            return CreateActionResult(await _cryptoCoinPriceService.CryptoCoinPrice());
        }
    }
}
