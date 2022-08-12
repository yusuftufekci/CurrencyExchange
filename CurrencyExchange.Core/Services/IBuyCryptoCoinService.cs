﻿using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Requests;

namespace CurrencyExchange.Core.Services
{
    public interface IBuyCryptoCoinService<T> where T : class
    {
        Task<CustomResponseDto<NoContentDto>> BuyCryptoCoinByUsdt(BuyCoinRequest buyCoinRequest, string token);

        Task<CustomResponseDto<NoContentDto>> BuyCryptoCoinByCoin(BuyCoinRequest buyCoinRequest, string token);

    }
}
