using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.CryptoCoins;
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
    public class AccountService<T> : IAccount<T> where T : class
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ICryptoCoinRepository _cryptoCoinRepository;
        private readonly ISenderLogger _sender;


        public AccountService(IUserRepository repository, IUnitOfWork unitOfWork, IAccountRepository accountRepository,
            ITokenRepository tokenRepository, IUserBalanceHistoryRepository userBalanceHistoryRepository,
            IBalanceRepository balanceRepository,
            ICryptoCoinRepository cryptoCoinRepository,ISenderLogger senderLogger)
        {
            _userRepository = repository;
            _UnitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _tokenRepository = tokenRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _cryptoCoinRepository = cryptoCoinRepository;
            _sender = senderLogger;
        }
        public async Task<CustomResponseDto<NoContentDto>> CreateAccount(CreateAccountRequest createAccountRequest)
        {
            var userExist = await _userRepository.Where(p => p.UserEmail == createAccountRequest.UserEmail).SingleOrDefaultAsync();
            Account tempAccount = new Account();
            var accountExist = await _accountRepository.Where(p => p.AccountName == createAccountRequest.AccountName).SingleOrDefaultAsync();
            if (accountExist != null)
            {
                _sender.SenderFunction("Log", "CreateAccount request failed. Account name already exist");
                return CustomResponseDto<NoContentDto>.Fail(401, "Account name already in use");
            }
          
            tempAccount.AccountName = createAccountRequest.AccountName;
            tempAccount.UserId = userExist.Id;
            await _accountRepository.AddAsync(tempAccount);
            await _UnitOfWork.CommitAsync();
            _sender.SenderFunction("Log", "DepositFunds request succesfully completed");
            return CustomResponseDto<NoContentDto>.Succes(201);
            
        }

    public async Task<CustomResponseDto<NoContentDto>> DepositFunds(DepositFundRequest createAccountRequest)
        {
            Balance tempBalance = new Balance();
            UserBalanceHistory tempUserBalanceHistory = new UserBalanceHistory();
            var accountExist = await _accountRepository.Where(p => p.User.UserEmail == createAccountRequest.UserEmail).SingleOrDefaultAsync();
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "DepositFunds request failed. Account not found");
                return CustomResponseDto<NoContentDto>.Fail(404,"Account not found");
            }

            var balanceExist = await _balanceRepository.Where(p => p.Account == accountExist && p.CryptoCoinId==3).SingleOrDefaultAsync();
            if (balanceExist == null)
            {
                tempUserBalanceHistory.Account = accountExist;
                tempUserBalanceHistory.MessageForChanging = createAccountRequest.TotalBalance + " USDT deposit into the account";
                tempUserBalanceHistory.ChangedAmount = createAccountRequest.TotalBalance;
                tempUserBalanceHistory.BoughtCryptoCoin = "USDT";
                tempUserBalanceHistory.SoldCryptoCoin = "USDT";

                var usdt = await _cryptoCoinRepository.Where(p => p.CoinName == "USDT").SingleOrDefaultAsync();
                tempBalance.CryptoCoin = usdt;
                tempBalance.Account = accountExist;
                tempBalance.TotalBalance = createAccountRequest.TotalBalance;
                // tempBalance.CryptoCoinId = 3;

                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _balanceRepository.AddAsync(tempBalance);
                await _UnitOfWork.CommitAsync();
                _sender.SenderFunction("Log", "DepositFunds request succesfully completed");

                return CustomResponseDto<NoContentDto>.Succes(201);
            }
            else
            {
                tempUserBalanceHistory.Account = accountExist;
                tempUserBalanceHistory.MessageForChanging = createAccountRequest.TotalBalance + " USDT deposit into the account";
                tempUserBalanceHistory.ChangedAmount = createAccountRequest.TotalBalance;
                tempUserBalanceHistory.BoughtCryptoCoin = "USDT";
                tempUserBalanceHistory.SoldCryptoCoin = "USDT";

                balanceExist.TotalBalance += createAccountRequest.TotalBalance;
                // tempBalance.CryptoCoinId = 3;

                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _UnitOfWork.CommitAsync();
                _sender.SenderFunction("Log", "DepositFunds request succesfully completed");
                return CustomResponseDto<NoContentDto>.Succes(201);
            }
        }
    }
}
