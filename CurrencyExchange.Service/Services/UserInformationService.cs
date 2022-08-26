using CurrencyExchange.Caching.CryptoCoins;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Services;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.Entities.Log;
using CurrencyExchange.Core.Entities.LogMessages;

namespace CurrencyExchange.Service.Services
{
    public class UserInformationService : IUserInformationService
    {
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ISenderLogger _logSender;
        private readonly ICommonFunctions _commonFunctions;
        private readonly CryptoCoinServiceWithCaching _cryptoCoinServiceWithCaching;


        public UserInformationService(IUserBalanceHistoryRepository userBalanceHistoryRepository,
            IBalanceRepository balanceRepository,
            ISenderLogger logSender  , ICommonFunctions commonFunctions, CryptoCoinServiceWithCaching cryptoCoinServiceWithCaching)
        {
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _logSender = logSender;
            _commonFunctions = commonFunctions;
            _cryptoCoinServiceWithCaching = cryptoCoinServiceWithCaching;
        }
        public async Task<CustomResponseDto<UserInformationDto>> GetUserInformation(string token)
        {
            ResponseMessages responseMessage;
            LogMessages logMessages;
            var user = await _commonFunctions.GetUser(token);

            var account = await _commonFunctions.GetAccount(token);

            if (account == null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("GetUserInformationAccountNotFound", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage("AccountNotFound", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<UserInformationDto>.Fail(404, responseMessage.Value);
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
            logMessages = await _commonFunctions.GetLogResponseMessage("GetUserInformationSuccess", language: "en");

            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<UserInformationDto>.Success(201, userInformations);

        }

        public async Task<CustomResponseDto<List<UserTransactionHistoryDto>>> GetUserTransactions(string token)
        {
            LogMessages logMessages;
            var account = await _commonFunctions.GetAccount(token);
            if (account == null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("GetUserTransactionsAccountNotFound", language: "en");
                var responseMessage = await _commonFunctions.GetApiResponseMessage("AccountNotFound", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<List<UserTransactionHistoryDto>>.Fail(404, responseMessage.Value);
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
            logMessages = await _commonFunctions.GetLogResponseMessage("GetUserTransactionsSuccess", language: "en");

            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<List<UserTransactionHistoryDto>>.Success(201, userTransactionHistories);
        }

        public async Task<CustomResponseDto<List<BalanceDto>>> GetUserBalanceInformation(string token)
        {
            LogMessages logMessages;
            var account = await _commonFunctions.GetAccount(token);
            if (account == null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("GetUserBalanceInformationAccountNotFound", language: "en");
                var responseMessage = await _commonFunctions.GetApiResponseMessage("AccountNotFound", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<List<BalanceDto>>.Fail(404, responseMessage.Value);
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
            logMessages = await _commonFunctions.GetLogResponseMessage("GetUserBalanceInformationSuccess", language: "en");

            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<List<BalanceDto>>.Success(201, userBalancesInfos);
        }
    }
}
