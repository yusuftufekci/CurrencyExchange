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
        private  readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ICryptoCoinRepository _cryptoCoinRepository;
        private readonly ISenderLogger _sender;


        public AccountService(IUserRepository repository, IUnitOfWork unitOfWork, IAccountRepository accountRepository,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
            IBalanceRepository balanceRepository,
            ICryptoCoinRepository cryptoCoinRepository,ISenderLogger senderLogger, ITokenRepository tokenRepository)
        {
            _userRepository = repository;
            _UnitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _cryptoCoinRepository = cryptoCoinRepository;
            _sender = senderLogger;
            _tokenRepository = tokenRepository;
        }
        public async Task<CustomResponseDto<NoContentDto>> CreateAccount(CreateAccountRequest createAccountRequest, string token)
        {
            var tokenExists = await _tokenRepository.Where(p => p.Token == token).SingleOrDefaultAsync();
            
            var userExist = await _userRepository.Where(p => p.Id == tokenExists.UserId).SingleOrDefaultAsync();

            var accountExist = await _accountRepository.Where(p => p.User == userExist).FirstOrDefaultAsync();
            if (accountExist != null)
            {
                _sender.SenderFunction("Log", "CreateAccount request failed. Account already exist");
                return CustomResponseDto<NoContentDto>.Fail(401, "Account already exist");
            }
            Account tempAccount = new Account
            {
                AccountName = createAccountRequest.AccountName,
                UserId = userExist.Id
            };
            
            await _accountRepository.AddAsync(tempAccount);
            await _UnitOfWork.CommitAsync();
            _sender.SenderFunction("Log", "DepositFunds request succesfully completed");
            return CustomResponseDto<NoContentDto>.Succes(201);
            
        }

    public async Task<CustomResponseDto<NoContentDto>> DepositFunds(DepositFundRequest createAccountRequest, string token)
        {
            var tokenExists = await _tokenRepository.Where(p => p.Token == token).SingleOrDefaultAsync();

            var userExist = await _userRepository.Where(p => p.Id == tokenExists.UserId).SingleOrDefaultAsync();
            var accountExist = await _accountRepository.Where(p => p.User == userExist).SingleOrDefaultAsync();
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "DepositFunds request failed. Account not found");
                return CustomResponseDto<NoContentDto>.Fail(404,"Account not found");
            }

            var balanceExist = await _balanceRepository.Where(p => p.Account == accountExist && p.CryptoCoinId==3).SingleOrDefaultAsync();
            if (balanceExist == null)
            {
                UserBalanceHistory tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = accountExist,
                    MessageForChanging = createAccountRequest.TotalBalance + " USDT deposit into the account",
                    ChangedAmount = createAccountRequest.TotalBalance,
                    BoughtCryptoCoin = "USDT",
                    SoldCryptoCoin = "USDT",
                    ChangedAmountSoldCryptoCoin = createAccountRequest.TotalBalance
                };

                var usdt = await _cryptoCoinRepository.Where(p => p.CoinName == "USDT").SingleOrDefaultAsync();
                Balance tempBalance = new Balance
                {
                    CryptoCoin = usdt,
                    Account = accountExist,
                    TotalBalance = createAccountRequest.TotalBalance
                };

                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _balanceRepository.AddAsync(tempBalance);
                await _UnitOfWork.CommitAsync();
                _sender.SenderFunction("Log", "DepositFunds request succesfully completed");

                return CustomResponseDto<NoContentDto>.Succes(201);
            }
            else
            {
                UserBalanceHistory tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = accountExist,
                    MessageForChanging = createAccountRequest.TotalBalance + " USDT deposit into the account",
                    ChangedAmount = createAccountRequest.TotalBalance,
                    BoughtCryptoCoin = "USDT",
                    SoldCryptoCoin = "USDT",
                    ChangedAmountSoldCryptoCoin = createAccountRequest.TotalBalance
                };

        

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
