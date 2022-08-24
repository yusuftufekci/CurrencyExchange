using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Requests;

namespace CurrencyExchange.Core.Services
{
    public interface IAccountService<T> where T : class
    {
        Task<CustomResponseDto<NoContentDto>> CreateAccount(CreateAccountRequest createAccountRequest, string token);

        Task<CustomResponseDto<NoContentDto>> DepositFunds(DepositFundRequest createAccountRequesttoken, string token);


    }
}
