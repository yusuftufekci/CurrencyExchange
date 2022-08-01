using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.UserInformationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInformationController : CustomBaseController
    {
        private readonly IUserInformationService<UserInformationRequest> _service;
        public UserInformationController(IUserInformationService<UserInformationRequest> service)
        {
            _service = service;
        }

        [HttpPost("AllUserInformation")]
        public async Task<IActionResult> GetAllUserInformation(UserInformationRequest userInformationRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.GetUserInformation(userInformationRequest));
        }

        [HttpPost("AllTransactionsOfuser")]
        public async Task<IActionResult> GetAllTranstactionsOfUser(UserInformationRequest userInformationRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.GetUserTranstactions(userInformationRequest));
        }


        [HttpPost("BalanceInformationOfUser")]
        public async Task<IActionResult> GetAllBalancesOfUser(UserInformationRequest userInformationRequest, [FromHeader] string token)
        {
            return CreateActionResult(await _service.GetUserBalanceInformation(userInformationRequest));
        }
    }
}
