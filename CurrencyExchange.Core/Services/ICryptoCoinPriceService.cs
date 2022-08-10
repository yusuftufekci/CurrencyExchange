using CurrencyExchange.Core.DTOs;

namespace CurrencyExchange.Core.Services
{
    public  interface ICryptoCoinPriceService
    {
        Task<CustomResponseDto<NoContentDto>> CryptoCoinPrice();

    }
}
