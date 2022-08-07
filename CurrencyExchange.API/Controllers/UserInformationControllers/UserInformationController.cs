using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.UserInformationControllers
{
    public class UserInformationController : CustomBaseController
    {
        private readonly IUserInformationService<UserInformationRequest> _service;
        public UserInformationController(IUserInformationService<UserInformationRequest> service)
        {
            _service = service;
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        // [ServiceFilter(typeof(NotFoundFilter<User>))]
        [HttpPost("AllUserInformation")]
        public async Task<IActionResult> GetAllUserInformation(UserInformationRequest userInformationRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.GetUserInformation(userInformationRequest, token));
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        //[ServiceFilter(typeof(NotFoundFilter<User>))]
        [HttpPost("AllTransactionsOfuser")]
        public async Task<IActionResult> GetAllTransactionsOfUser(UserInformationRequest userInformationRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.GetUserTranstactions(userInformationRequest, token));
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        //[ServiceFilter(typeof(NotFoundFilter<User>))]
        [HttpPost("BalanceInformationOfUser")]
        public async Task<IActionResult> GetAllBalancesOfUser(UserInformationRequest userInformationRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.GetUserBalanceInformation(userInformationRequest, token));
        }
    }
}
