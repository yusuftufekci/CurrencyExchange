using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.RabbitMqLogger;
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

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [ServiceFilter(typeof(NotFoundFilter<User>))]
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateUser(CreateAccountRequest createAccountRequest, [FromHeader] string token)
        {

            return CreateActionResult(await _service.CreateAccount(createAccountRequest));
        }

        [ServiceFilter(typeof(TokenControlFilter<UserToken>))]
        [ServiceFilter(typeof(NotFoundFilter<User>))]
        [HttpPost("DepositFund")]
        public async Task<IActionResult> DepositFund(DepositFundRequest depositFundRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.DepositFunds(depositFundRequest));
        }

        //[HttpPost("RabbitMqDeneme")]
        //public async Task<IActionResult> Rabbit(DepositFundRequest depositFundRequest)
        //{
        //    _sender.SenderFUnction();

        //    string response =_recieveLogger.RecieveFunction();

        //    _logger.LogInfo("Here is info message from the controller.");
        //    _logger.LogDebug("Here is debug message from the controller.");
        //    _logger.LogWarn("Here is warn message from the controller.");
        //    _logger.LogError("Here is error message from the controller.");

        //    return CreateActionResult(CustomResponseDto<string>.Succes(201,response));

        //}
    }
}
