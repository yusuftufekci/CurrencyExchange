using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.UserInformationControllers
{
    public class UserInformationController : ControllerBase
    {
        private readonly IUserInformationService _service;
        public UserInformationController(IUserInformationService service)
        {
            _service = service;
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("all-user-information")]
        public async Task<CustomResponseDto<UserInformationDto>> GetAllUserInformation( [FromHeader] string token)
        {
            return (await _service.GetUserInformation(token));
        }
        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("all-transactions-of-user")]
        public async Task<CustomResponseDto<List<UserTransactionHistoryDto>>> GetAllTransactionsOfUser( [FromHeader] string token)
        {
            return (await _service.GetUserTransactions( token));
        }
        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("balance-information-of-user")]
        public async Task<CustomResponseDto<List<BalanceDto>>> GetAllBalancesOfUser( [FromHeader] string token)
        {
            return await _service.GetUserBalanceInformation( token);
        }
    }
}
