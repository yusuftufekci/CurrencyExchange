using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.CryptoCoins;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Service.Services
{
    public class CryptoCoinPriceService : ICryptoCoinPriceService
    {
        private readonly ICryptoCoinPriceRepository _cryptoCoinPriceRepository;
        private readonly IUnitOfWork _UnitOfWork;

        public CryptoCoinPriceService(IUnitOfWork unitOfWork,
          ICryptoCoinPriceRepository cryptoCoinPriceRepository , IUnitOfWork unitOfWork1  ) 
        {
            _UnitOfWork = unitOfWork1;
            _cryptoCoinPriceRepository = cryptoCoinPriceRepository;
            
        }

        public async Task<CustomResponseDto<NoContentDto>> CryptoCoinPrice()
        {

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(1);
                HttpResponseMessage response = await client.GetAsync("https://api.binance.com/api/v3/ticker/price");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var ResponceString = await response.Content.ReadAsStringAsync();

                    var cryptoCoinPrices =  _cryptoCoinPriceRepository.GetAll().ToList();

                    var ResponceObject = JsonConvert.DeserializeObject<List<CryptoCoinPriceDto>>(ResponceString);
                    if (cryptoCoinPrices.Count == 0)
                    {
                        foreach (var item in ResponceObject)
                        {
                            var coinPrice = new CryptoCoinPrice();

                            coinPrice.Price = item.price;
                            coinPrice.Symbol = item.symbol;
                            _cryptoCoinPriceRepository.AddAsync(coinPrice);
                        }
                        await _UnitOfWork.CommitAsync();
                        return CustomResponseDto<NoContentDto>.Succes(201);
                    }
                    else
                    {
                        foreach (var item in ResponceObject)
                        {
                           var coinPrice2 =  _cryptoCoinPriceRepository.GetAll().ToList();
                            if (coinPrice2 == null)
                            {
                                var coinPrice = new CryptoCoinPrice();

                                coinPrice.Price = item.price;
                                coinPrice.ModifiedDate = DateTime.UtcNow;
                            }

                        }
                        await _UnitOfWork.CommitAsync();
                        return CustomResponseDto<NoContentDto>.Succes(201);

                    }
                }
                else
                {
                    return CustomResponseDto<NoContentDto>.Fail(404,"Problem");

                }
            }


        }
    

    }
}
