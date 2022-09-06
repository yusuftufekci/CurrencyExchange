using System.Net;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.ConfigModels;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.Log;
using CurrencyExchange.Core.Entities.LogMessages;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CurrencyExchange.Caching.CryptoCoins;
using CurrencyExchange.Core.ConstantsMessages;
using CurrencyExchange.Service.LogFacade;

namespace CurrencyExchange.Service.Services
{
    public class BuyCryptoCoinService : IBuyCryptoCoinService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ISenderLogger _logSender;
        private readonly ICommonFunctions _commonFunctions;
        private readonly ControlCryptoCoinAmountSettings _controlCryptoCoinAmountSettings;
        private readonly CryptoCoinPriceServiceWithCaching _cryptoCoinPriceServiceWithCaching;
        private readonly CryptoCoinServiceWithCaching _cryptoCoinServiceWithCaching;
        private readonly LogResponseFacade _logResponseFacade;


        public BuyCryptoCoinService( IUnitOfWork unitOfWork,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
           IBalanceRepository balanceRepository, ISenderLogger logSender ,
            ICommonFunctions commonFunctions, IOptions<ControlCryptoCoinAmountSettings> controlCryptoCoinAmountSettings,
            CryptoCoinPriceServiceWithCaching cryptoCoinPriceServiceWithCaching, CryptoCoinServiceWithCaching cryptoCoinServiceWithCaching, LogResponseFacade logResponseFacade)
        {
            _unitOfWork = unitOfWork;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _logSender = logSender;
            _commonFunctions = commonFunctions;
            _controlCryptoCoinAmountSettings = controlCryptoCoinAmountSettings.Value;
            _cryptoCoinPriceServiceWithCaching = cryptoCoinPriceServiceWithCaching;
            _cryptoCoinServiceWithCaching = cryptoCoinServiceWithCaching;
            _logResponseFacade = logResponseFacade;
        }

        public double CalculateTotalAmountByUsdt(string price, double amount)
        {
            var coinPrice = Convert.ToDouble(price);
            var totalAmount = coinPrice * amount;
            totalAmount = Math.Round(totalAmount, _controlCryptoCoinAmountSettings.NumberOfRound);
            return totalAmount;
        }

        public double CalculateTotalAmountByCoin(string price, double amount)
        {
            var coinPrice = Convert.ToDouble(price);
            var totalAmount = amount / coinPrice;
            totalAmount = Math.Round(totalAmount, _controlCryptoCoinAmountSettings.NumberOfRound);
            return totalAmount;
        }


        public async Task<CustomResponseDto<NoContentDto>> BuyCryptoCoinByUsdt(BuyCoinRequest buyCoinRequest, string token)
        {

            ResponseMessages responseMessage;
            var account = await _commonFunctions.GetAccount(token);
            var symbolOfCoins = buyCoinRequest.CoinToBuy + buyCoinRequest.BuyWIthThisCoin;
            if (account == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountAccountNotFound, ConstantResponseMessage.AccountNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var balance = await _balanceRepository.Where(p => p.CryptoCoinName == buyCoinRequest.BuyWIthThisCoin && p.Account == account).SingleOrDefaultAsync();
            if (balance == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountBalanceNotFound, ConstantResponseMessage.BalanceNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var cryptoCoinPrices = _cryptoCoinPriceServiceWithCaching.GetCryptoCoinPrice();
            var coinTypeToBuy = cryptoCoinPrices.SingleOrDefault(p => p.Symbol == symbolOfCoins);
            if (coinTypeToBuy == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountPriceNotFound, ConstantResponseMessage.BalanceNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var totalAmount = CalculateTotalAmountByUsdt(coinTypeToBuy.Price, buyCoinRequest.Amount);
            if (totalAmount <= _controlCryptoCoinAmountSettings.TotalAmount)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountLowPrice, ConstantResponseMessage.LowAmount, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.BadRequest, responseMessage.Value);
            }
            if (totalAmount > balance.TotalBalance)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountLowPriceOfCoin, ConstantResponseMessage.LowAmountOfCoin, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.BadRequest, responseMessage.Value);
            }
            var balanceForBuyCoin = await _balanceRepository.Where(p => p.Account == account && p.CryptoCoinName == buyCoinRequest.CoinToBuy).SingleOrDefaultAsync();
            var cryptoCoins = _cryptoCoinServiceWithCaching.GetCryptoCoins();
            if (cryptoCoins == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountApiFailed, ConstantResponseMessage.ApiProblem, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);

            }
            var coinToBuy = cryptoCoins.SingleOrDefault(p => p.CoinName == buyCoinRequest.CoinToBuy);
            if (balanceForBuyCoin == null)
            {
                var tempBalance = new Balance
                {
                    CryptoCoinName = coinToBuy.CoinName,
                    Account = account,
                    TotalBalance = buyCoinRequest.Amount
                };
                balance.TotalBalance -= totalAmount;
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = account,
                    MessageForChanging = buyCoinRequest.Amount + " " + buyCoinRequest.CoinToBuy + " deposit into the account",
                    ChangedAmount = buyCoinRequest.Amount,
                    BoughtCryptoCoin = buyCoinRequest.CoinToBuy,
                    SoldCryptoCoin = buyCoinRequest.BuyWIthThisCoin,
                    ChangedAmountSoldCryptoCoin = totalAmount
                };
                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _balanceRepository.AddAsync(tempBalance);
            }
            else
            {
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = account,
                    MessageForChanging = buyCoinRequest.Amount + " " + buyCoinRequest.CoinToBuy + " deposit into the account",
                    ChangedAmount = buyCoinRequest.Amount,
                    BoughtCryptoCoin = buyCoinRequest.CoinToBuy,
                    SoldCryptoCoin = buyCoinRequest.BuyWIthThisCoin,
                    ChangedAmountSoldCryptoCoin = totalAmount
                };
                balanceForBuyCoin.TotalBalance += buyCoinRequest.Amount;
                balance.TotalBalance -= totalAmount;
                balance.ModifiedDate = DateTime.UtcNow;
                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            }
            await _unitOfWork.CommitAsync();
            var logMessages = await _commonFunctions.GetLogResponseMessage(ConstantLogMessages.BuyCoinWithAmountSuccess, language: "en");
            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<NoContentDto>.Success();

        }

        public async Task<CustomResponseDto<NoContentDto>> BuyCryptoCoinByCoin(BuyCoinRequest buyCoinRequest, string token)
        {
            ResponseMessages responseMessage;
            var account = await _commonFunctions.GetAccount(token);
            var symbolOfCoins = buyCoinRequest.CoinToBuy + buyCoinRequest.BuyWIthThisCoin;
            if (account == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountAccountNotFound, ConstantResponseMessage.AccountNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var balance = await _balanceRepository.Where(p => p.CryptoCoinName == buyCoinRequest.BuyWIthThisCoin && p.Account == account).SingleOrDefaultAsync();
            if (balance == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountBalanceNotFound, ConstantResponseMessage.BalanceNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var cryptoCoinPrices = _cryptoCoinPriceServiceWithCaching.GetCryptoCoinPrice();
            var coinTypeToBuy = cryptoCoinPrices.SingleOrDefault(p => p.Symbol == symbolOfCoins);
            if (coinTypeToBuy == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountPriceNotFound, ConstantResponseMessage.BalanceNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }

            var totalAmount = CalculateTotalAmountByCoin(coinTypeToBuy.Price, buyCoinRequest.Amount);
            if (totalAmount <= _controlCryptoCoinAmountSettings.TotalAmount)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountLowPrice, ConstantResponseMessage.LowAmount, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.BadRequest, responseMessage.Value);
            }

            if (totalAmount > balance.TotalBalance)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountLowPriceOfCoin, ConstantResponseMessage.LowAmountOfCoin, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.BadRequest, responseMessage.Value);
            }
            var balanceForBuyCoin = await _balanceRepository.Where(p => p.Account == account && p.CryptoCoinName == buyCoinRequest.CoinToBuy).SingleOrDefaultAsync();
            var cryptoCoins = _cryptoCoinServiceWithCaching.GetCryptoCoins();
            if (cryptoCoins == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.BuyCoinWithAmountApiFailed, ConstantResponseMessage.ApiProblem, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var coinToBuy = cryptoCoins.SingleOrDefault(p => p.CoinName == buyCoinRequest.CoinToBuy); if (balanceForBuyCoin == null)
            {
                var tempBalance = new Balance
                {
                    CryptoCoinName = coinToBuy.CoinName,
                    Account = account,
                    TotalBalance = totalAmount
                };
                balance.TotalBalance -= buyCoinRequest.Amount;
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = account,
                    MessageForChanging = totalAmount + " " + buyCoinRequest.CoinToBuy + " deposit into the account",
                    ChangedAmount = totalAmount,
                    BoughtCryptoCoin = buyCoinRequest.CoinToBuy,
                    SoldCryptoCoin = buyCoinRequest.BuyWIthThisCoin,
                    ChangedAmountSoldCryptoCoin = buyCoinRequest.Amount
                };
                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _balanceRepository.AddAsync(tempBalance);
            }
            else
            {
                balanceForBuyCoin.TotalBalance += totalAmount;
                balance.TotalBalance -= buyCoinRequest.Amount;
                balance.ModifiedDate = DateTime.UtcNow;
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = account,
                    MessageForChanging = totalAmount + " " + buyCoinRequest.CoinToBuy + " deposit into the account",
                    ChangedAmount = totalAmount,
                    BoughtCryptoCoin = buyCoinRequest.CoinToBuy,
                    SoldCryptoCoin = buyCoinRequest.BuyWIthThisCoin,
                    ChangedAmountSoldCryptoCoin = buyCoinRequest.Amount
                };
                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            }
            await _unitOfWork.CommitAsync();
            var logMessages = await _commonFunctions.GetLogResponseMessage(ConstantLogMessages.BuyCoinWithAmount2Success, language: "en");
            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<NoContentDto>.Success();

        }
    }
}
