using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.TransactionControllers
{
 
    public class SellCryptoCoinController : CustomBaseController
    {
        private readonly ISellCryptoCoinService _service;
        public SellCryptoCoinController(ISellCryptoCoinService service)
        {
            _service = service;
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("sell-crypto-coin")]
        public async Task<IActionResult> SellCryptoCoin(SellCryptoCoinRequest sellCryptoCoinRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.SellCryptoCoin(sellCryptoCoinRequest, token));
        }
        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("sell-crypto-coin2")]
        public async Task<IActionResult> SellCryptoCoinV2(SellCryptoCoinRequest sellCryptoCoinRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.SellCryptoCoinV2(sellCryptoCoinRequest, token));
        }
    }
}
