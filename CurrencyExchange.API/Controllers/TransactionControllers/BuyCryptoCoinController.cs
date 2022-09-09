using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.TransactionControllers
{

    public class BuyCryptoCoinController : ControllerBase
    {
        private readonly IBuyCryptoCoinService _service;
        public BuyCryptoCoinController(IBuyCryptoCoinService service)
        {
            _service = service;
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("buy-crypto-coin")]
        public async Task<CustomResponseDto<NoContentDto>> BuyCryptoCoin(BuyCoinRequest buyCoinRequest, [FromHeader] string token)
        {
            return await _service.BuyCryptoCoinByUsdt(buyCoinRequest, token);
        }


        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("buy-crypto-coin2")]
        public async Task<CustomResponseDto<NoContentDto>> BuyCryptoCoinV2(BuyCoinRequest buyCoinRequest, [FromHeader] string token)
        {
            return await _service.BuyCryptoCoinByCoin(buyCoinRequest, token);
        }


    }
}
