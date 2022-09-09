using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.AccountControllers
{
    
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;
        public AccountController(IAccountService service)
        {
            _service = service;
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("create-account")]
        public async Task<CustomResponseDto<NoContentDto>> CreateUser(CreateAccountRequest createAccountRequest, [FromHeader] string token)
        {
            return (await _service.CreateAccount(createAccountRequest,token));
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [HttpPost("deposit-fund")]
        public async Task<CustomResponseDto<NoContentDto>> DepositFund(DepositFundRequest depositFundRequest, [FromHeader] string token)
        {
            return (await _service.DepositFunds(depositFundRequest, token));
        }
    }
}
