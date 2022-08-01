using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.AuthenticationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : CustomBaseController
    {
        private readonly IUserRegister<UserRegisterRequest> _service;
        public AuthenticationController(IUserRegister<UserRegisterRequest> service)
        {
            _service = service;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser(UserRegisterRequest userRegisterRequest)
        { 
            userRegisterRequest.IpAdress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return CreateActionResult(await _service.UserRegister(userRegisterRequest));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginRequest userLoginRequest)
        {

            userLoginRequest.IpAdress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return CreateActionResult(await _service.UserLogin(userLoginRequest));
        }

    }

}
