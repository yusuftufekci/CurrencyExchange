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
        private readonly IUserRegister<UserRegisterRequest> _service;
        public AuthenticationController(IUserRegister<UserRegisterRequest> service)
        {
            _service = service;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser([FromBody]UserRegisterRequest userRegisterRequest)
        { 
            string IpAdress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return CreateActionResult(await _service.UserRegister(userRegisterRequest, IpAdress));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginRequest userLoginRequest)
        {

            string IpAdress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return CreateActionResult(await _service.UserLogin(userLoginRequest, IpAdress));
        }

    }

}
