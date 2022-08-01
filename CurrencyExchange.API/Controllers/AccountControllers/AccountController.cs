using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.AccountControllers
{
    
    public class AccountController : CustomBaseController
    {
        private readonly IAccount<CreateAccountRequest> _service;

        public AccountController(IAccount<CreateAccountRequest> service)
        {
            _service = service;
        }
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateUser(CreateAccountRequest createAccountRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.CreateAccount(createAccountRequest, token));
        }
        [HttpPost("DepositFund")]

        public async Task<IActionResult> DepositFund(DepositFundRequest depositFundRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.DepositFunds(depositFundRequest, token));
        }
    }
}
