using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.TransactionControllers
{

    public class BuyCryptoCoinController : CustomBaseController
    {
        private readonly IBuyCryptoCoinService<BuyCoinRequest> _service;
        public BuyCryptoCoinController(IBuyCryptoCoinService<BuyCoinRequest> service)
        {
            _service = service;
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [ServiceFilter(typeof(NotFoundFilter<User>))]
        [HttpPost("BuyCryptoCoin")]
        public async Task<IActionResult> BuyCryptoCoin(BuyCoinRequest buyCoinRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.BuyCoinWithAmount(buyCoinRequest));
        }


        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [ServiceFilter(typeof(NotFoundFilter<User>))]
        [HttpPost("BuyCryptoCoinV2")]
        public async Task<IActionResult> BuyCryptoCoinV2(BuyCoinRequest buyCoinRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.BuyCoinWithAmount2(buyCoinRequest));
        }


    }
}
