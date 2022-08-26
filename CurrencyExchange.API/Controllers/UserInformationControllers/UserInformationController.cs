using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.UserInformationControllers
{
    public class UserInformationController : CustomBaseController
    {
        private readonly IUserInformationService _service;
        public UserInformationController(IUserInformationService service)
        {
            _service = service;
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("AllUserInformation")]
        public async Task<IActionResult> GetAllUserInformation( [FromHeader] string token)
        {
            return CreateActionResult(await _service.GetUserInformation(token));
        }
        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("AllTransactionsOfuser")]
        public async Task<IActionResult> GetAllTransactionsOfUser( [FromHeader] string token)
        {
            return CreateActionResult(await _service.GetUserTransactions( token));
        }
        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("BalanceInformationOfUser")]
        public async Task<IActionResult> GetAllBalancesOfUser( [FromHeader] string token)
        {
            return CreateActionResult(await _service.GetUserBalanceInformation( token));
        }
    }
}
