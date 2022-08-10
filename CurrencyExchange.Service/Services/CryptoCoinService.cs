using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.CryptoCoins;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Service.Services
{
    public class CryptoCoinService : ICryptoCoinService
    {
        private readonly ICryptoCoinRepository _cryptoCoinRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISenderLogger _sender;

        public CryptoCoinService(IUnitOfWork unitOfWork,
          ICryptoCoinRepository cryptoCoinRepository, IUnitOfWork unitOfWork1, ISenderLogger sender)
        {
            _unitOfWork = unitOfWork1;
            _cryptoCoinRepository = cryptoCoinRepository;
            _sender = sender;
        }
        public async Task<CustomResponseDto<NoContentDto>> CryptoCoin()
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(1);
                HttpResponseMessage response = await client.GetAsync("https://api.nomics.com/v1/currencies/ticker?key=ab9543f3c307afc219e1b55e9527559447536691");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var cryptoCoins = _cryptoCoinRepository.GetAll().ToList();
                    var responceString = await response.Content.ReadAsStringAsync();
                    var root = (JContainer)JToken.Parse(responceString);
                    var list = root.DescendantsAndSelf().OfType<JProperty>().Where(p => p.Name == "id").Select(p => p.Value.Value<string>());
                    if (cryptoCoins == null)
                    {
                        foreach (var item in list)
                        {
                            var tempCoinType = new CryptoCoin
                            {
                                CoinName = item
                            };
                            await _cryptoCoinRepository.AddAsync(tempCoinType);
                        }
                    }
                    else
                    {
                        foreach (var item in list)
                        {

                            var coinExist =  _cryptoCoinRepository.Where(p => p.CoinName == item).SingleOrDefault();
                            if (coinExist == null)
                            {
                                var tempCoinType = new CryptoCoin
                                {
                                    CoinName = item
                                };
                                await _cryptoCoinRepository.AddAsync(tempCoinType);
                            }
                        }
                    }
                }
                else
                {
                    _sender.SenderFunction("Log", "CryptoCoinPrice request failed. Connection Problem.");
                    return CustomResponseDto<NoContentDto>.Fail(404,"Problem");
                }
            }
            await _unitOfWork.CommitAsync();
            _sender.SenderFunction("Log", "CryptoCoin request succesfully completed.");
            return CustomResponseDto<NoContentDto>.Succes(201);
        }
    }
}
