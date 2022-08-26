using CurrencyExchange.Core.DTOs;

namespace CurrencyExchange.Core.Services
{
    public interface IUserInformationService
    {
        Task<CustomResponseDto<UserInformationDto>> GetUserInformation( string token);

        Task<CustomResponseDto<List<UserTransactionHistoryDto>>> GetUserTransactions(string token);

        Task<CustomResponseDto<List<BalanceDto>>> GetUserBalanceInformation( string token);



    }
}
