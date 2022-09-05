using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.Caching.CryptoCoins;
using CurrencyExchange.Core.CommonFunction;
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

namespace CurrencyExchange.Service.Services
{
    public class CancellationService : ICancellationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ISenderLogger _logSender;
        private readonly ICommonFunctions _commonFunctions;
        private readonly CryptoCoinServiceWithCaching _cryptoCoinServiceWithCaching;
        private readonly LogResponseFacade _logResponseFacade;

        public CancellationService(IUnitOfWork unitOfWork, IAccountRepository accountRepository, IUserBalanceHistoryRepository userBalanceHistoryRepository, IBalanceRepository balanceRepository, ISenderLogger logSender, CryptoCoinServiceWithCaching cryptoCoinServiceWithCaching, LogResponseFacade logResponseFacade, ICommonFunctions commonFunctions)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _logSender = logSender;
            _cryptoCoinServiceWithCaching = cryptoCoinServiceWithCaching;
            _logResponseFacade = logResponseFacade;
            _commonFunctions = commonFunctions;
        }
        public async Task<CustomResponseDto<NoContentDto>> RollBack(CancellationRequest cancellationRequest, string token)
        {
            ResponseMessages responseMessage;
            var account = await _commonFunctions.GetAccount(token);
            if (account == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage("GetUserTransactionsAccountNotFound", ConstantResponseMessage.AccountNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var userTransaction = await _userBalanceHistoryRepository.Where(p => p.Id == cancellationRequest.TransactionHistoryId).SingleAsync();
            if (userTransaction == null)
            {
                responseMessage = await _logResponseFacade.GetLogAndResponseMessage(
                    "GetUserTransactionsAccountNotFound", ConstantResponseMessage.AccountNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }

            if (userTransaction.BoughtCryptoCoin == "USDT" && userTransaction.SoldCryptoCoin == "USDT")
            {
                var usdtUserBalanceHistory = new UserBalanceHistory
                {
                    Account = account,
                    MessageForChanging = "The transaction with id number" + cancellationRequest.TransactionHistoryId + " has been rolled back.",
                    ChangedAmount = -Math.Abs(userTransaction.ChangedAmount),
                    BoughtCryptoCoin = "USDT",
                    SoldCryptoCoin = "USDT",
                    ChangedAmountSoldCryptoCoin = Math.Abs(userTransaction.ChangedAmount)
                };
                var usdtBalance = await _balanceRepository.Where(p => p.Account == account && p.CryptoCoinName == "USDT").SingleOrDefaultAsync();

                usdtBalance.TotalBalance -= userTransaction.ChangedAmount;
                await _userBalanceHistoryRepository.AddAsync(usdtUserBalanceHistory);
            }
            var soldCoinBalance = await _balanceRepository.Where(p => p.Account == account && p.CryptoCoinName == userTransaction.SoldCryptoCoin).SingleOrDefaultAsync();
            var boughtCoinBalance = await _balanceRepository.Where(p => p.Account == account && p.CryptoCoinName == userTransaction.BoughtCryptoCoin).SingleOrDefaultAsync();
            soldCoinBalance.TotalBalance += userTransaction.ChangedAmountSoldCryptoCoin;
            boughtCoinBalance.TotalBalance -= userTransaction.ChangedAmount;
            var coinUserBalanceHistory = new UserBalanceHistory
            {
                Account = account,
                MessageForChanging = "The transaction with id number" + cancellationRequest.TransactionHistoryId + " has been rolled back.",
                ChangedAmount = userTransaction.ChangedAmountSoldCryptoCoin,
                BoughtCryptoCoin = userTransaction.SoldCryptoCoin,
                SoldCryptoCoin = userTransaction.BoughtCryptoCoin,
                ChangedAmountSoldCryptoCoin = userTransaction.ChangedAmount
            };
            await _userBalanceHistoryRepository.AddAsync(coinUserBalanceHistory);
            await _unitOfWork.CommitAsync(); ;
            return CustomResponseDto<NoContentDto>.Success();
        }
    }
}
