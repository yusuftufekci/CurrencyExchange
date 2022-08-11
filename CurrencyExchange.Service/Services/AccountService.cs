using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.HelperFunctions;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Service.Services
{
    public class AccountService<T> : IAccount<T> where T : class
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ISenderLogger _sender;
        private readonly ICommonFunctions _commonFunctions;


        public AccountService(IUserRepository repository, IUnitOfWork unitOfWork, IAccountRepository accountRepository,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
            IBalanceRepository balanceRepository,
            ISenderLogger senderLogger, ITokenRepository tokenRepository, ICommonFunctions commonFunctions)
        {
            _userRepository = repository;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _sender = senderLogger;
            _tokenRepository = tokenRepository;
            _commonFunctions = commonFunctions;
        }
        public async Task<CustomResponseDto<NoContentDto>> CreateAccount(CreateAccountRequest createAccountRequest, string token)
        {
            var userExist = await _commonFunctions.GetUser(token);
            var accountExist = await _commonFunctions.GetAccount(token);

            if (accountExist != null)
            {
                _sender.SenderFunction("Log", "CreateAccount request failed. Account already exist");
                return CustomResponseDto<NoContentDto>.Fail(401, "Account already exist");
            }
            var tempAccount = new Account
            {
                AccountName = createAccountRequest.AccountName,
                UserId = userExist.Id
            };
            await _accountRepository.AddAsync(tempAccount);
            await _unitOfWork.CommitAsync();
            _sender.SenderFunction("Log", "DepositFunds request successfully completed");
            return CustomResponseDto<NoContentDto>.Succes(201);
        }

        public async Task<CustomResponseDto<NoContentDto>> DepositFunds(DepositFundRequest createAccountRequest, string token)
        {
            var accountExist = await _commonFunctions.GetAccount(token);
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "DepositFunds request failed. Account not found");
                return CustomResponseDto<NoContentDto>.Fail(404, "Account not found");
            }
            var balanceExist = await _balanceRepository.Where(p => p.Account == accountExist && p.CryptoCoinName == "USDT").SingleOrDefaultAsync();
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

                var cryptoCoins = await GetCryptoCoins.AsyncGetCryptoCoins();

                var usdt = cryptoCoins.SingleOrDefault(p => p.CoinName == "USDT");
                var tempBalance = new Balance
                {
                    CryptoCoinName = usdt.CoinName,
                    Account = accountExist,
                    TotalBalance = createAccountRequest.TotalBalance
                };

                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _balanceRepository.AddAsync(tempBalance);
                await _unitOfWork.CommitAsync();
                _sender.SenderFunction("Log", "DepositFunds request successfully completed");

                return CustomResponseDto<NoContentDto>.Succes(201);
            }
            else
            {
                var tempUserBalanceHistory = new UserBalanceHistory
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
                await _unitOfWork.CommitAsync();
                _sender.SenderFunction("Log", "DepositFunds request successfully completed");
                return CustomResponseDto<NoContentDto>.Succes(201);
            }
        }
    }
}
