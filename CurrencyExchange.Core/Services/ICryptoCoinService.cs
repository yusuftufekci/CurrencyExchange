using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.CryptoCoins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Services
{
    public interface ICryptoCoinService
    {
        Task<CustomResponseDto<NoContentDto>> CryptoCoin();
    }
}
