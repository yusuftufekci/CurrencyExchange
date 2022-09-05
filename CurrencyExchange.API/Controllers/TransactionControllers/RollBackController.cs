using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.TransactionControllers
{
    public class RollBackController : CustomBaseController
    {
        private readonly ICancellationService _service;
        public RollBackController(ICancellationService service)
        {
            _service = service;
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("roll-back-transaction")]
        public async Task<IActionResult> BuyCryptoCoin(CancellationRequest cancellationRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.RollBack(cancellationRequest, token));
        }
    }
}
