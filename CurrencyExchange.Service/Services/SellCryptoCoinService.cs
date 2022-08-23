using CurrencyExchange.Cachgin;
using CurrencyExchange.Caching;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.ConfigModels;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.Log;
using CurrencyExchange.Core.Entities.LogMessages;
using CurrencyExchange.Core.HelperFunctions;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CurrencyExchange.Service.Services
{
    public class SellCryptoCoinService<T> : ISellCryptoCoinService<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ISenderLogger _logSender;
        private readonly ICommonFunctions _commonFunctions;
        private readonly ControlCryptoCoinAmountSettings _controlCryptoCoinAmountSettings;
        private readonly CryptoCoinPriceServiceWithCaching _cryptoCoinPriceServiceWithCaching;

        public SellCryptoCoinService( IUnitOfWork unitOfWork, 
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
           IBalanceRepository balanceRepository,
            ISenderLogger logSender, ICommonFunctions commonFunctions, IOptions<ControlCryptoCoinAmountSettings> controlCryptoCoinAmountSettings, CryptoCoinPriceServiceWithCaching cryptoCoinPriceServiceWithCaching )
        {
            _unitOfWork = unitOfWork;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _logSender = logSender;
            _commonFunctions = commonFunctions;
            _cryptoCoinPriceServiceWithCaching = cryptoCoinPriceServiceWithCaching;
            _controlCryptoCoinAmountSettings = controlCryptoCoinAmountSettings.Value;

        }

        public async Task<CustomResponseDto<NoContentDto>> SellCryptoCoin(SellCryptoCoinRequest sellCryptoCoinRequest, string token)
        {
            /*
             Bu fonksiyon kullanıcı eğer belli bir miktar  Coinlik satış yapmak isterse kullanılacak.
            */
            ResponseMessages responseMessage;
            LogMessages logMessages;
            var account = await _commonFunctions.GetAccount(token);

            var symbolOfCoins = sellCryptoCoinRequest.CoinToSell + "USDT";
            if (account == null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinAccountNotFound", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage("AccountNotFound", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            var cryptoCoinPrices = _cryptoCoinPriceServiceWithCaching.GetCryptoCoinPrice();
            var coinTypeToBuy = cryptoCoinPrices.SingleOrDefault(p => p.Symbol == symbolOfCoins); if (coinTypeToBuy == null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinPriceNotFound", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage("BalanceNotFound", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            var balance = await _balanceRepository.Where(p => p.CryptoCoinName == sellCryptoCoinRequest.CoinToSell && p.Account == account).SingleOrDefaultAsync();
            if (balance == null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinBalanceNotFound", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage("BalanceNotFound", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            var coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            var totalAmount = coinPrice * sellCryptoCoinRequest.Amount;
            totalAmount = Math.Round(totalAmount, _controlCryptoCoinAmountSettings.NumberOfRound);
            if (totalAmount <=_controlCryptoCoinAmountSettings.TotalAmount)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinLowPrice", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage("LowAmount", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            if (sellCryptoCoinRequest.Amount > balance.TotalBalance)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinLowPriceOfCoin", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage("LowAmountOfCoin", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            var balanceForBuyCoin = await _balanceRepository.Where(p => p.CryptoCoinName == "USDT" && p.Account == account).SingleOrDefaultAsync();
            balanceForBuyCoin.TotalBalance += totalAmount;
            balance.TotalBalance -= sellCryptoCoinRequest.Amount;
            balance.ModifiedDate = DateTime.UtcNow;
            var tempUserBalanceHistory = new UserBalanceHistory
            {
                Account = account,
                MessageForChanging = sellCryptoCoinRequest.Amount + " " + sellCryptoCoinRequest.CoinToSell + " sold. It's equal to = " + totalAmount + " USDT",
                ChangedAmount = sellCryptoCoinRequest.Amount,
                BoughtCryptoCoin = "USDT",
                SoldCryptoCoin = sellCryptoCoinRequest.CoinToSell
            };
            await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            await _unitOfWork.CommitAsync();
            logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinSuccess", language: "en");

            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<NoContentDto>.Succes(201);

        }

        public async Task<CustomResponseDto<NoContentDto>> SellCryptoCoinV2(SellCryptoCoinRequest sellCryptoCoinRequest, string token)
        {
            /*
             Bu fonksiyon kullanıcı eğer belli bir miktar dolarlık satış yapmak isterse kullanılacak.
            */
            ResponseMessages responseMessage;
            LogMessages logMessages;
            var account = await _commonFunctions.GetAccount(token);
            var symbolOfCoins = sellCryptoCoinRequest.CoinToSell + "USDT";
            if (account == null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinAccountNotFound", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage("AccountNotFound", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            var cryptoCoinPrices = _cryptoCoinPriceServiceWithCaching.GetCryptoCoinPrice();
            var coinTypeToBuy = cryptoCoinPrices.SingleOrDefault(p => p.Symbol == symbolOfCoins); if (coinTypeToBuy == null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinPriceNotFound", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage("BalanceNotFound", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            var balance = await _balanceRepository.Where(p => p.CryptoCoinName == sellCryptoCoinRequest.CoinToSell && p.Account == account).SingleOrDefaultAsync();
            if (balance == null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinBalanceNotFound", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage("BalanceNotFound", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            var coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            var totalAmount = sellCryptoCoinRequest.Amount / coinPrice; //Coinden  çıkaracağım miktar
            totalAmount = Math.Round(totalAmount, _controlCryptoCoinAmountSettings.NumberOfRound);
            if (totalAmount <= _controlCryptoCoinAmountSettings.TotalAmount)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinLowPrice", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage("LowAmount", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            if (totalAmount > balance.TotalBalance)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinLowPriceOfCoin", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage("LowAmountOfCoin", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            var balanceForBuyCoin = await _balanceRepository.Where(p => p.CryptoCoinName == "USDT" && p.Account == account).SingleOrDefaultAsync();
            balanceForBuyCoin.TotalBalance += sellCryptoCoinRequest.Amount;
            balance.TotalBalance -= totalAmount;
            balance.ModifiedDate = DateTime.UtcNow;
            var tempUserBalanceHistory = new UserBalanceHistory
            {
                Account = account,
                MessageForChanging = totalAmount + " " + sellCryptoCoinRequest.CoinToSell + " sold. It's equal to = " + sellCryptoCoinRequest.Amount + " USDT",
                ChangedAmount = sellCryptoCoinRequest.Amount,
                BoughtCryptoCoin = "USDT",
                SoldCryptoCoin = sellCryptoCoinRequest.CoinToSell
            };
            await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            await _unitOfWork.CommitAsync();
            logMessages = await _commonFunctions.GetLogResponseMessage("SellCryptoCoinSuccess", language: "en");

            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<NoContentDto>.Succes(201);
        }
    }
}
