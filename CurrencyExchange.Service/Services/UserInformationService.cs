using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using CurrencyExchange.Service.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.HelperFunctions;

namespace CurrencyExchange.Service.Services
{
    public class UserInformationService<T> : IUserInformationService<T> where T : class
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ISenderLogger _sender;
        private readonly ICommonFunctions _commonFunctions;

        public UserInformationService(IAccountRepository accountRepository,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
            IBalanceRepository balanceRepository,
            ISenderLogger sender, ITokenRepository tokenRepository, IUserRepository userRepository, ICommonFunctions commonFunctions)
        {
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _sender = sender;
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _commonFunctions = commonFunctions;
        }
        public async Task<CustomResponseDto<UserInformationDto>> GetUserInformation(string token)
        {
            var userExist = await _commonFunctions.GetUser(token);

            var accountExist = await _commonFunctions.GetAccount(token);

            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "CreateAccount request failed. Account not found");
                return CustomResponseDto<UserInformationDto>.Fail(404, "Account not found");
            }
            var balances = await _balanceRepository.Where(p => p.Account == accountExist).ToListAsync();
            var userBalancesInfos = new List<BalanceDto>();
            var cryptoCoins = await GetCryptoCoins.AsyncGetCryptoCoins();

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
                UserAccountName = accountExist.AccountName,
                UserEmail = userExist.UserEmail
            };
            _sender.SenderFunction("Log", "GetUserInformation request successfully completed");
            return CustomResponseDto<UserInformationDto>.Succes(201, userInformations);

        }

        public async Task<CustomResponseDto<List<UserTransactionHistoryDto>>> GetUserTransactions(string token)
        {
            var accountExist = await _commonFunctions.GetAccount(token);
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "GetUserTransactions request failed. Account not found");
                return CustomResponseDto<List<UserTransactionHistoryDto>>.Fail(404, "Account not found");
            }
            var userTransactionHistories = new List<UserTransactionHistoryDto>();
            var transactions = await _userBalanceHistoryRepository.Where(p => p.Account == accountExist).ToListAsync();
            foreach (var item in transactions)
            {
                var userTransactions = new UserTransactionHistoryDto
                {
                    MessageForChanging = item.MessageForChanging,
                    AccountName = accountExist.AccountName,
                    ChangedAmount = item.ChangedAmount,
                    BoughtCryptoCoin = item.BoughtCryptoCoin,
                    SoldCryptoCoin = item.SoldCryptoCoin
                };
                userTransactionHistories.Add(userTransactions);
            }
            _sender.SenderFunction("Log", "GetUserTransactions request succesfully completed");
            return CustomResponseDto<List<UserTransactionHistoryDto>>.Succes(201, userTransactionHistories);
        }

        public async Task<CustomResponseDto<List<BalanceDto>>> GetUserBalanceInformation(string token)
        {
            var accountExist = await _commonFunctions.GetAccount(token);
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "GetUserBalanceInformation request failed. Account not found");
                return CustomResponseDto<List<BalanceDto>>.Fail(404, "Account not found");
            }
            var balances = await _balanceRepository.Where(p => p.Account == accountExist).ToListAsync();
            var userBalancesInfos = new List<BalanceDto>();
            var cryptoCoins = await GetCryptoCoins.AsyncGetCryptoCoins();

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
            _sender.SenderFunction("Log", "GetUserBalanceInformation request successfully completed");
            return CustomResponseDto<List<BalanceDto>>.Succes(201, userBalancesInfos);
        }
    }
}
