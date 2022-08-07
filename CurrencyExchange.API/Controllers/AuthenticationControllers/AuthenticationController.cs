using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.AuthenticationControllers
{
    public class AuthenticationController : CustomBaseController
    {
        private readonly IAuthenticationService<UserRegisterRequest> _service;
        public AuthenticationController(IAuthenticationService<UserRegisterRequest> service)
        {
            _service = service;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser([FromBody]UserRegisterRequest userRegisterRequest)
        { 
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return CreateActionResult(await _service.UserRegister(userRegisterRequest, ipAddress));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginRequest userLoginRequest)
        {
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return CreateActionResult(await _service.UserLogin(userLoginRequest, ipAddress));
        }

    }

}
