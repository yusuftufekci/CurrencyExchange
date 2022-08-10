using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Requests;

namespace CurrencyExchange.Core.Services
{
    public interface IBuyCryptoCoinService<T> where T : class
    {
        Task<CustomResponseDto<NoContentDto>> BuyCoinWithAmount(BuyCoinRequest buyCoinRequest, string token);

        Task<CustomResponseDto<NoContentDto>> BuyCoinWithAmount2(BuyCoinRequest buyCoinRequest, string token);

    }
}
