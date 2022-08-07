using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.TransactionControllers
{
 
    public class SellCryptoCoinController : CustomBaseController
    {
        private readonly ISellCryptoCoinService<SellCryptoCoinRequest> _service;
        public SellCryptoCoinController(ISellCryptoCoinService<SellCryptoCoinRequest> service)
        {
            _service = service;
        }


        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        //[ServiceFilter(typeof(NotFoundFilter<User>))]
        [HttpPost("SellCryptoCoin")]
        public async Task<IActionResult> SellCryptoCoin(SellCryptoCoinRequest sellCryptoCoinRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.SellCryptoCoin(sellCryptoCoinRequest, token));
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        //[ServiceFilter(typeof(NotFoundFilter<User>))]
        [HttpPost("SellCryptoCoinV2")]
        public async Task<IActionResult> SellCryptoCoinV2(SellCryptoCoinRequest sellCryptoCoinRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.SellCryptoCoinV2(sellCryptoCoinRequest, token));
        }
    }
}
