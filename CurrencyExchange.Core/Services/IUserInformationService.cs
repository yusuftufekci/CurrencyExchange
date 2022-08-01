using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Services
{
    public interface IUserInformationService<T> where T : class
    {
        Task<CustomResponseDto<UserInformationDto>> GetUserInformation(UserInformationRequest userInformationRequest);

        Task<CustomResponseDto<List<UserTransactionHistoryDto>>> GetUserTranstactions(UserInformationRequest userInformationRequest);

        Task<CustomResponseDto<List<BalanceDto>>> GetUserBalanceInformation(UserInformationRequest userInformationRequest);



    }
}
