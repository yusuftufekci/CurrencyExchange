using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Services
{
    public interface IUserRegister<T> where T : class

    {
        Task<CustomResponseDto<NoContentDto>> UserRegister(UserRegisterRequest userRegisterRequest, string IpAdress);
        Task<CustomResponseDto<TokenDto>> UserLogin(UserLoginRequest userLoginRequest, string IpAdress);


    }
}
