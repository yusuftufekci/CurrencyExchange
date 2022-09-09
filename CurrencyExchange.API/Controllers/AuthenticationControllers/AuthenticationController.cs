using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers.AuthenticationControllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _service;
        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<CustomResponseDto<NoContentDto>> CreateUser(UserRegisterRequest userRegisterRequest)
        { 
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return await _service.UserRegister(userRegisterRequest, ipAddress);
        }

        [HttpPost("login")]
        public async Task<CustomResponseDto<TokenDto>> Login(UserLoginRequest userLoginRequest)
        {
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return await _service.UserLogin(userLoginRequest, ipAddress);
        }

    }

}
