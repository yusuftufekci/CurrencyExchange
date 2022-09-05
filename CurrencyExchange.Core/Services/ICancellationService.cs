using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Requests;

namespace CurrencyExchange.Core.Services
{
    public interface ICancellationService
    {
        Task<CustomResponseDto<NoContentDto>> RollBack(CancellationRequest cancellationRequest, string token);

    }
}
