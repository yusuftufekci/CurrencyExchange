using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.TransactionControllers
{
    public class RollBackController : ControllerBase
    {
        private readonly ICancellationService _service;
        public RollBackController(ICancellationService service)
        {
            _service = service;
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("roll-back-transaction")]
        public async Task<CustomResponseDto<NoContentDto>> BuyCryptoCoin(CancellationRequest cancellationRequest, [FromHeader] string token)
        {
            return await _service.RollBack(cancellationRequest, token);
        }
    }
}
