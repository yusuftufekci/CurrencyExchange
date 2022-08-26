using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Requests;

namespace CurrencyExchange.Core.Services
{
    public interface IAuthenticationService

    {
        Task<CustomResponseDto<NoContentDto>> UserRegister(UserRegisterRequest userRegisterRequest, string ipAdress);
        Task<CustomResponseDto<TokenDto>> UserLogin(UserLoginRequest userLoginRequest, string ipAddress);


    }
}
