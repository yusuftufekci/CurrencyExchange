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

namespace CurrencyExchange.Service.Services
{
    public class UserInformationService<T> : IUserInformationService<T> where T : class
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ICryptoCoinRepository _cryptoCoinRepository;
        private readonly ISenderLogger _sender;

        public UserInformationService( IAccountRepository accountRepository,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
            IBalanceRepository balanceRepository, ICryptoCoinRepository cryptoCoinRepository,
            ISenderLogger sender, ITokenRepository tokenRepository,IUserRepository userRepository )
        {
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _cryptoCoinRepository = cryptoCoinRepository;
            _sender = sender;
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;

        }
        public async Task<CustomResponseDto<UserInformationDto>> GetUserInformation( string token)
        {
            var tokenExists = await _tokenRepository.Where(p => p.Token == token).SingleOrDefaultAsync();
            var userExist = await _userRepository.Where(p => p.Id == tokenExists.UserId).SingleOrDefaultAsync();
            var accountExist = await _accountRepository.Where(p => p.User.UserEmail == userExist.UserEmail).SingleOrDefaultAsync();
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "CreateAccount request failed. Account not found");
                return CustomResponseDto<UserInformationDto>.Fail(404, "Account not found");
            }
            var balances = await _balanceRepository.Where(p => p.Account == accountExist).ToListAsync();
            var userBalancesInfos = new List<BalanceDto>();
            foreach (var item in balances)
            {
                var tempCoin = await _cryptoCoinRepository.Where(p => p.Id == item.CryptoCoinId).SingleOrDefaultAsync();

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

        public async Task<CustomResponseDto<List<UserTransactionHistoryDto>>> GetUserTransactions( string token)
        {
            var tokenExists = await _tokenRepository.Where(p => p.Token == token).SingleOrDefaultAsync();
            var userExist = await _userRepository.Where(p => p.Id == tokenExists.UserId).SingleOrDefaultAsync();
            var accountExist = await _accountRepository.Where(p => p.User.UserEmail == userExist.UserEmail).SingleOrDefaultAsync();
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

        public async Task<CustomResponseDto<List<BalanceDto>>> GetUserBalanceInformation( string token)
        {
            var tokenExists = await _tokenRepository.Where(p => p.Token == token).SingleOrDefaultAsync();
            var userExist = await _userRepository.Where(p => p.Id == tokenExists.UserId).SingleOrDefaultAsync();
            var accountExist = await _accountRepository.Where(p => p.User.UserEmail == userExist.UserEmail).SingleOrDefaultAsync();
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "GetUserBalanceInformation request failed. Account not found");
                return CustomResponseDto<List<BalanceDto>>.Fail(404, "Account not found");
            }
            var balances = await _balanceRepository.Where(p => p.Account == accountExist).ToListAsync();
            var userBalancesInfos = new List<BalanceDto>();
            foreach (var item in balances)
            {
                var tempCoin = await _cryptoCoinRepository.Where(p => p.Id == item.CryptoCoinId).SingleOrDefaultAsync();
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
