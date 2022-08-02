using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Services
{
    public interface IBuyCryptoCoinService<T> where T : class
    {
        Task<CustomResponseDto<NoContentDto>> BuyCoinWithAmount(BuyCoinRequest buyCoinRequest);

        Task<CustomResponseDto<NoContentDto>> BuyCoinWithAmount2(BuyCoinRequest buyCoinRequest);

    }
}
