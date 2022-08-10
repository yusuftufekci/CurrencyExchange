using CurrencyExchange.Core.DTOs;

namespace CurrencyExchange.Core.Services
{
    public interface ICryptoCoinService
    {
        Task<CustomResponseDto<NoContentDto>> CryptoCoin();
    }
}
