using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.CryptoCoins;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Service.Services
{
    public class AccountService<T> : IAccount<T> where T : class
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ICryptoCoinRepository _cryptoCoinRepository;


        public AccountService(IUserRepository repository, IUnitOfWork unitOfWork, IAccountRepository accountRepository,
            ITokenRepository tokenRepository, IUserBalanceHistoryRepository userBalanceHistoryRepository,
            IBalanceRepository balanceRepository, ICryptoCoinRepository cryptoCoinRepository)
        {
            _userRepository = repository;
            _UnitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _tokenRepository = tokenRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _cryptoCoinRepository = cryptoCoinRepository;
        }
        public async Task<CustomResponseDto<NoContentDto>> CreateAccount(CreateAccountRequest createAccountRequest, string token)
        {
            var userExist = await _userRepository.Where(p => p.UserEmail == createAccountRequest.UserEmail).SingleOrDefaultAsync();
            if (userExist == null)
            {
                return CustomResponseDto<NoContentDto>.Fail(404,"Email address not found");
            }
            Account tempAccount = new Account();

            tempAccount.AccountName = createAccountRequest.AccountName;
            tempAccount.UserId = userExist.Id;
            await _accountRepository.AddAsync(tempAccount);
            await _UnitOfWork.CommitAsync();
            return CustomResponseDto<NoContentDto>.Succes(201);
        }

        public async Task<CustomResponseDto<NoContentDto>> DepositFunds(DepositFundRequest createAccountRequest, string token)
        {
            Balance tempBalance = new Balance();
            UserBalanceHistory tempUserBalanceHistory = new UserBalanceHistory();
            var userExist = await _userRepository.Where(p => p.UserEmail == createAccountRequest.UserEmail).SingleOrDefaultAsync();
            if (userExist == null)
            {
                return CustomResponseDto<NoContentDto>.Fail(404, "Email address not found");
            }
            var accountExist = await _accountRepository.Where(p => p.UserId == userExist.Id).SingleOrDefaultAsync();
            if (accountExist == null)
            {
                return CustomResponseDto<NoContentDto>.Fail(404, "Account Not found");
            }
            tempUserBalanceHistory.Account = accountExist;
            tempUserBalanceHistory.MessageForChanging = createAccountRequest.TotalBalance + " USDT deposit into the account";
            tempUserBalanceHistory.ChangedAmount = createAccountRequest.TotalBalance;
            tempUserBalanceHistory.ExchangedCoinName = "USDT";

            var usdt = await _cryptoCoinRepository.Where(p => p.CoinName == "USDT").SingleOrDefaultAsync();
            tempBalance.CryptoCoin = usdt;
            tempBalance.Account = accountExist;
            tempBalance.TotalBalance = createAccountRequest.TotalBalance;
            // tempBalance.CryptoCoinId = 3;

            await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            await _balanceRepository.AddAsync(tempBalance);
            await _UnitOfWork.CommitAsync();
            return CustomResponseDto<NoContentDto>.Succes(201);
        }
    }
}
