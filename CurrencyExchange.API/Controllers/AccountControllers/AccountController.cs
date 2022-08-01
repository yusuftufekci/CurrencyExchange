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
        public async Task<IActionResult> CreateUser(CreateAccountRequest createAccountRequest)
        {
            return CreateActionResult(await _service.CreateAccount(createAccountRequest));
        }
        [HttpPost("DepositFund")]

        public async Task<IActionResult> DepositFund(DepositFundRequest depositFundRequest)
        {
            return CreateActionResult(await _service.DepositFunds(depositFundRequest));
        }
    }
}
