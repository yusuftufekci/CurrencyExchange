using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.TransactionControllers
{

    public class BuyCryptoCoinController : CustomBaseController
    {
        private readonly IBuyCryptoCoinService _service;
        public BuyCryptoCoinController(IBuyCryptoCoinService service)
        {
            _service = service;
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("buy-crypto-coin")]
        public async Task<IActionResult> BuyCryptoCoin(BuyCoinRequest buyCoinRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.BuyCryptoCoinByUsdt(buyCoinRequest, token));
        }


        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("buy-crypto-coin2")]
        public async Task<IActionResult> BuyCryptoCoinV2(BuyCoinRequest buyCoinRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.BuyCryptoCoinByCoin(buyCoinRequest, token));
        }


    }
}
