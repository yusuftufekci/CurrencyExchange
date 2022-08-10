﻿using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Requests;

namespace CurrencyExchange.Core.Services
{
    public interface ISellCryptoCoinService<T> where T : class
    {
        Task<CustomResponseDto<NoContentDto>> SellCryptoCoin(SellCryptoCoinRequest sellCryptoCoinRequest, string token);

        Task<CustomResponseDto<NoContentDto>> SellCryptoCoinV2(SellCryptoCoinRequest sellCryptoCoinRequest, string token);
    }
}
