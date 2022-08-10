using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.CryptoCoins;
using CurrencyExchange.Core.RabbitMqLogger;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISenderLogger _sender;

        public CryptoCoinPriceService(IUnitOfWork unitOfWork,
          ICryptoCoinPriceRepository cryptoCoinPriceRepository, IUnitOfWork unitOfWork1, ISenderLogger senderLogger)
        {
            _unitOfWork = unitOfWork1;
            _cryptoCoinPriceRepository = cryptoCoinPriceRepository;
            _sender = senderLogger;

        }

        public async Task<CustomResponseDto<NoContentDto>> CryptoCoinPrice()
        {

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(1);
                HttpResponseMessage response = await client.GetAsync("https://api.binance.com/api/v3/ticker/price");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responceString = await response.Content.ReadAsStringAsync();

                    var cryptoCoinPrices = _cryptoCoinPriceRepository.GetAll().ToList();

                    var responceObject = JsonConvert.DeserializeObject<List<CryptoCoinPriceDto>>(responceString);
                    if (cryptoCoinPrices.Count == 0)
                    {
                        foreach (var item in responceObject)
                        {
                            var coinPrice = new CryptoCoinPrice();

                            coinPrice.Price = item.Price;
                            coinPrice.Symbol = item.Symbol;
                            _cryptoCoinPriceRepository.AddAsync(coinPrice);
                        }
                        await _unitOfWork.CommitAsync();
                        _sender.SenderFunction("Log", "CryptoCoinPrice request succesfully completed.");

                        return CustomResponseDto<NoContentDto>.Succes(201);
                    }
                    else
                    {
                        foreach (var item in responceObject)
                        {
                            var coinPrice2 = _cryptoCoinPriceRepository.GetAll().ToList();
                            if (coinPrice2 == null)
                            {
                                var coinPrice = new CryptoCoinPrice();

                                coinPrice.Price = item.Price;
                                coinPrice.ModifiedDate = DateTime.UtcNow;
                            }

                        }
                        await _unitOfWork.CommitAsync();
                        _sender.SenderFunction("Log", "CryptoCoinPrice request succesfully completed.");
                        return CustomResponseDto<NoContentDto>.Succes(201);

                    }
                }
                else
                {
                    _sender.SenderFunction("Log", "CryptoCoinPrice request failed. Connection Problem.");
                    return CustomResponseDto<NoContentDto>.Fail(404, "Problem");

                }
            }


        }


    }
}