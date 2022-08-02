using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Services
{
    public interface IAccount<T> where T : class
    {
        Task<CustomResponseDto<NoContentDto>> CreateAccount(CreateAccountRequest createAccountRequest);

        Task<CustomResponseDto<NoContentDto>> DepositFunds(DepositFundRequest createAccountRequesttoken);


    }
}
