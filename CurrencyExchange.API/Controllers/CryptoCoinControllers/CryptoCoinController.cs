//using CurrencyExchange.API.Filters;
//using CurrencyExchange.Core.Entities.Authentication;
//using CurrencyExchange.Core.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace CurrencyExchange.API.Controllers.CryptoCoinControllers
//{
//    public class CryptoCoinController : CustomBaseController
//    {
//        private readonly ICryptoCoinService _cryptoCoinService;
//        public CryptoCoinController(ICryptoCoinService cryptoCoinService )
//        {
//            _cryptoCoinService = cryptoCoinService;
//        }
//        [HttpPost("GetCryptoCoinList")]
//        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
//        public async Task<IActionResult> GetCryptoCoinList()
//        {
//            return CreateActionResult(await _cryptoCoinService.CryptoCoin());
//        }
//    }
//}
