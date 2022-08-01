using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.CryptoCoinControllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetCryptoCoinList()
        {
            return CreateActionResult(await _cryptoCoinService.CryptoCoin());
        }

        [HttpPost("GetCryptoCoinPriceList")]
        public async Task<IActionResult> GetCryptoCoinPriceList()
        {
            return CreateActionResult(await _cryptoCoinPriceService.CryptoCoinPrice());
        }
    }
}
