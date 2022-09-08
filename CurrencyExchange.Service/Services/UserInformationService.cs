using System.Net;
using CurrencyExchange.Caching.CryptoCoins;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Services;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.ConstantsMessages;
using CurrencyExchange.Service.LogFacade;
using CurrencyExchange.Core.Constants;

namespace CurrencyExchange.Service.Services
{
    public class UserInformationService : IUserInformationService
    {
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ISenderLogger _logSender;
        private readonly ICommonFunctions _commonFunctions;
        private readonly CryptoCoinServiceWithCaching _cryptoCoinServiceWithCaching;
        private readonly LogResponseFacade _logResponseFacade;



        public UserInformationService(IUserBalanceHistoryRepository userBalanceHistoryRepository,
            IBalanceRepository balanceRepository,
            ISenderLogger logSender  , ICommonFunctions commonFunctions, CryptoCoinServiceWithCaching cryptoCoinServiceWithCaching, LogResponseFacade logResponseFacade)
        {
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _logSender = logSender;
            _commonFunctions = commonFunctions;
            _cryptoCoinServiceWithCaching = cryptoCoinServiceWithCaching;
            _logResponseFacade = logResponseFacade;
        }
        public async Task<CustomResponseDto<UserInformationDto>> GetUserInformation(string token)
        {
            var user = await _commonFunctions.GetUser(token);

            var account = await _commonFunctions.GetAccount(token);

            if (account == null)
            {
                var responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.GetUserInformationAccountNotFound, ConstantResponseMessage.AccountNotFound, Language.English);
                return CustomResponseDto<UserInformationDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var balances = await _balanceRepository.Where(p => p.Account == account).ToListAsync();
            var userBalancesInfos = new List<BalanceDto>();
            var cryptoCoins = _cryptoCoinServiceWithCaching.GetCryptoCoins();

            foreach (var item in balances)
            {
                var tempCoin = cryptoCoins.SingleOrDefault(p => p.CoinName == item.CryptoCoinName);


                var userBalancesInfo = new BalanceDto
                {
                    TotalAmount = item.TotalBalance,
                    CryptoCoinName = tempCoin.CoinName

                };
                userBalancesInfos.Add(userBalancesInfo);
            }
            var userInformations = new UserInformationDto
            {
                UserBalances = userBalancesInfos,
                UserAccountName = account.AccountName,
                UserEmail = user.UserEmail
            };
            var logMessages = await _commonFunctions.GetLogResponseMessage(ConstantLogMessages.GetUserInformationSuccess, language: Language.English);

            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<UserInformationDto>.Success(userInformations);

        }

        public async Task<CustomResponseDto<List<UserTransactionHistoryDto>>> GetUserTransactions(string token)
        {
            var account = await _commonFunctions.GetAccount(token);
            if (account == null)
            {
                var responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.GetUserTransactionsAccountNotFound, ConstantResponseMessage.AccountNotFound, Language.English);
                return CustomResponseDto<List<UserTransactionHistoryDto>>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var userTransactionHistories = new List<UserTransactionHistoryDto>();
            var transactions = await _userBalanceHistoryRepository.Where(p => p.Account == account).ToListAsync();
            foreach (var item in transactions)
            {
                var userTransactions = new UserTransactionHistoryDto
                {
                    MessageForChanging = item.MessageForChanging,
                    AccountName = account.AccountName,
                    ChangedAmount = item.ChangedAmount,
                    BoughtCryptoCoin = item.BoughtCryptoCoin,
                    SoldCryptoCoin = item.SoldCryptoCoin
                };
                userTransactionHistories.Add(userTransactions);
            }
            var logMessages = await _commonFunctions.GetLogResponseMessage(ConstantLogMessages.GetUserTransactionsSuccess, language: Language.English);

            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<List<UserTransactionHistoryDto>>.Success(userTransactionHistories);
        }

        public async Task<CustomResponseDto<List<BalanceDto>>> GetUserBalanceInformation(string token)
        {
            var account = await _commonFunctions.GetAccount(token);
            if (account == null)
            {
                var responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.GetUserBalanceInformationAccountNotFound, ConstantResponseMessage.AccountNotFound, Language.English);
                return CustomResponseDto<List<BalanceDto>>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var balances = await _balanceRepository.Where(p => p.Account == account).ToListAsync();
            var userBalancesInfos = new List<BalanceDto>();
            var cryptoCoins = _cryptoCoinServiceWithCaching.GetCryptoCoins();

            foreach (var item in balances)
            {
                var tempCoin = cryptoCoins.SingleOrDefault(p => p.CoinName == item.CryptoCoinName);
                var userBalancesInfo = new BalanceDto
                {
                    TotalAmount = item.TotalBalance,
                    CryptoCoinName = tempCoin.CoinName
                };
                userBalancesInfos.Add(userBalancesInfo);
            }
            var logMessages = await _commonFunctions.GetLogResponseMessage(ConstantLogMessages.GetUserBalanceInformationSuccess, language: Language.English);

            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<List<BalanceDto>>.Success(userBalancesInfos);
        }
    }
}
