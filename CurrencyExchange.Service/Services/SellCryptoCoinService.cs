using System.Net;
using CurrencyExchange.Caching.CryptoCoins;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.ConfigModels;
using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.ConstantsMessages;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.Log;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using CurrencyExchange.Service.LogFacade;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CurrencyExchange.Service.Services
{
    public class SellCryptoCoinService : ISellCryptoCoinService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ISenderLogger _logSender;
        private readonly ICommonFunctions _commonFunctions;
        private readonly ControlCryptoCoinAmountSettings _controlCryptoCoinAmountSettings;
        private readonly CryptoCoinPriceServiceWithCaching _cryptoCoinPriceServiceWithCaching;
        private readonly LogResponseFacade _logResponseFacade;

        public SellCryptoCoinService( IUnitOfWork unitOfWork, 
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
           IBalanceRepository balanceRepository,
            ISenderLogger logSender, ICommonFunctions commonFunctions, IOptions<ControlCryptoCoinAmountSettings> controlCryptoCoinAmountSettings, CryptoCoinPriceServiceWithCaching cryptoCoinPriceServiceWithCaching, LogResponseFacade logResponseFacade)
        {
            _unitOfWork = unitOfWork;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _logSender = logSender;
            _commonFunctions = commonFunctions;
            _cryptoCoinPriceServiceWithCaching = cryptoCoinPriceServiceWithCaching;
            _logResponseFacade = logResponseFacade;
            _controlCryptoCoinAmountSettings = controlCryptoCoinAmountSettings.Value;

        }

        public async Task<CustomResponseDto<NoContentDto>> SellCryptoCoin(SellCryptoCoinRequest sellCryptoCoinRequest, string token)
        {
            /*
             Bu fonksiyon kullanıcı eğer belli bir miktar  Coinlik satış yapmak isterse kullanılacak.
            */
            ResponseMessages responseMessage;
            var account = await _commonFunctions.GetAccount(token);

            var symbolOfCoins = sellCryptoCoinRequest.CoinToSell + Usdt.Name;
            if (account == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.SellCryptoCoinAccountNotFound, ConstantResponseMessage.AccountNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);

            }
            var cryptoCoinPrices = _cryptoCoinPriceServiceWithCaching.GetCryptoCoinPrice();
            var coinTypeToBuy = cryptoCoinPrices.SingleOrDefault(p => p.Symbol == symbolOfCoins);
            if (coinTypeToBuy == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.SellCryptoCoinPriceNotFound, ConstantResponseMessage.BalanceNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            var balance = await _balanceRepository.Where(p => p.CryptoCoinName == sellCryptoCoinRequest.CoinToSell && p.Account == account).SingleOrDefaultAsync();
            if (balance == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.SellCryptoCoinBalanceNotFound, ConstantResponseMessage.BalanceNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            var totalAmount = coinPrice * sellCryptoCoinRequest.Amount;
            totalAmount = Math.Round(totalAmount, _controlCryptoCoinAmountSettings.NumberOfRound);
            if (totalAmount <=_controlCryptoCoinAmountSettings.TotalAmount)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.SellCryptoCoinLowPrice, ConstantResponseMessage.LowAmount, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.BadRequest, responseMessage.Value);
            }
            if (sellCryptoCoinRequest.Amount > balance.TotalBalance)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.SellCryptoCoinLowPriceOfCoin, ConstantResponseMessage.LowAmountOfCoin, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.BadRequest, responseMessage.Value);
            }
            var balanceForBuyCoin = await _balanceRepository.Where(p => p.CryptoCoinName == Usdt.Name && p.Account == account).SingleOrDefaultAsync();
            balanceForBuyCoin.TotalBalance += totalAmount;
            balance.TotalBalance -= sellCryptoCoinRequest.Amount;
            balance.ModifiedDate = DateTime.UtcNow;
            var tempUserBalanceHistory = new UserBalanceHistory
            {
                Account = account,
                MessageForChanging = TransactionHistoryMessages.SoldCoinMessage(sellCryptoCoinRequest.Amount, sellCryptoCoinRequest.CoinToSell, totalAmount),
                ChangedAmount = totalAmount,
                BoughtCryptoCoin = Usdt.Name,
                SoldCryptoCoin = sellCryptoCoinRequest.CoinToSell,
                ChangedAmountSoldCryptoCoin = sellCryptoCoinRequest.Amount
                
            };
            await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            await _unitOfWork.CommitAsync();
            var logMessages = await _commonFunctions.GetLogResponseMessage(ConstantLogMessages.SellCryptoCoinSuccess, language: "en");
            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<NoContentDto>.Success();

        }

        public async Task<CustomResponseDto<NoContentDto>> SellCryptoCoinV2(SellCryptoCoinRequest sellCryptoCoinRequest, string token)
        {
            /*
             Bu fonksiyon kullanıcı eğer belli bir miktar dolarlık satış yapmak isterse kullanılacak.
            */
            ResponseMessages responseMessage;
            var account = await _commonFunctions.GetAccount(token);
            var symbolOfCoins = sellCryptoCoinRequest.CoinToSell + Usdt.Name;
            if (account == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.SellCryptoCoinAccountNotFound, ConstantResponseMessage.AccountNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            var cryptoCoinPrices = _cryptoCoinPriceServiceWithCaching.GetCryptoCoinPrice();
            var coinTypeToBuy = cryptoCoinPrices.SingleOrDefault(p => p.Symbol == symbolOfCoins);
            if (coinTypeToBuy == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.SellCryptoCoinPriceNotFound, ConstantResponseMessage.BalanceNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }
            var balance = await _balanceRepository.Where(p => p.CryptoCoinName == sellCryptoCoinRequest.CoinToSell && p.Account == account).SingleOrDefaultAsync();
            if (balance == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.SellCryptoCoinBalanceNotFound, ConstantResponseMessage.BalanceNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            var totalAmount = sellCryptoCoinRequest.Amount / coinPrice; //Coinden  çıkaracağım miktar
            totalAmount = Math.Round(totalAmount, _controlCryptoCoinAmountSettings.NumberOfRound);
            if (totalAmount <= _controlCryptoCoinAmountSettings.TotalAmount)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.SellCryptoCoinLowPrice, ConstantResponseMessage.LowAmount, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.BadRequest, responseMessage.Value);
            }
            if (totalAmount > balance.TotalBalance)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.SellCryptoCoinLowPriceOfCoin, ConstantResponseMessage.LowAmountOfCoin, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.BadRequest, responseMessage.Value);
            }
            var balanceForBuyCoin = await _balanceRepository.Where(p => p.CryptoCoinName == Usdt.Name && p.Account == account).SingleOrDefaultAsync();
            balanceForBuyCoin.TotalBalance += sellCryptoCoinRequest.Amount;
            balance.TotalBalance -= totalAmount;
            balance.ModifiedDate = DateTime.UtcNow;
            var tempUserBalanceHistory = new UserBalanceHistory
            {
                Account = account,
                MessageForChanging = TransactionHistoryMessages.SoldCoinMessage(totalAmount, sellCryptoCoinRequest.CoinToSell, sellCryptoCoinRequest.Amount),
                ChangedAmount = sellCryptoCoinRequest.Amount,
                BoughtCryptoCoin = Usdt.Name,
                SoldCryptoCoin = sellCryptoCoinRequest.CoinToSell,
                ChangedAmountSoldCryptoCoin = totalAmount

            };
            await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            await _unitOfWork.CommitAsync();
            var logMessages = await _commonFunctions.GetLogResponseMessage(ConstantLogMessages.SellCryptoCoinSuccess, language: "en");

            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<NoContentDto>.Success();
        }
    }
}
