using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.LogMessages;
using CurrencyExchange.Core.HelperFunctions;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Service.Services
{
    public class AccountService<T> : IAccountService<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ISenderLogger _logSender;
        private readonly ICommonFunctions _commonFunctions;


        public AccountService(IUnitOfWork unitOfWork, IAccountRepository accountRepository,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
            IBalanceRepository balanceRepository,
            ISenderLogger logSenderLogger , ICommonFunctions commonFunctions)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _logSender = logSenderLogger;
            _commonFunctions = commonFunctions;
        }
        public async Task<CustomResponseDto<NoContentDto>> CreateAccount(CreateAccountRequest createAccountRequest, string token)
        {
            LogMessages logMessages;
            var user = await _commonFunctions.GetUser(token);
            var account = await _commonFunctions.GetAccount(token);

            if (account != null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("CreateAccountAccountAlreadyExist", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                var responseMessage = await _commonFunctions.GetApiResponseMessage("AccountAlreadyExist", language: "en");
                return CustomResponseDto<NoContentDto>.Fail(401, responseMessage.Value);
            }
            var tempAccount = new Account
            {
                AccountName = createAccountRequest.AccountName,
                UserId = user.Id
            };
            await _accountRepository.AddAsync(tempAccount);
            await _unitOfWork.CommitAsync();
            logMessages = await _commonFunctions.GetLogResponseMessage("CreateAccountSuccess", language: "en");
            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<NoContentDto>.Success(201);
        }

        public async Task<CustomResponseDto<NoContentDto>> DepositFunds(DepositFundRequest createAccountRequest, string token)
        {
            LogMessages logMessages;
            var account = await _commonFunctions.GetAccount(token);
            if (account == null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("DepositFundsAccountNotFound", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                var responseMessage = await _commonFunctions.GetApiResponseMessage("AccountNotFound", language: "en");
                return CustomResponseDto<NoContentDto>.Fail(404, responseMessage.Value);
            }

            var balance = await _balanceRepository.Where(p => p.Account == account && p.CryptoCoinName == "USDT").SingleOrDefaultAsync();
            if (balance == null)
            {
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = account,
                    MessageForChanging = createAccountRequest.TotalBalance + " USDT deposit into the account",
                    ChangedAmount = createAccountRequest.TotalBalance,
                    BoughtCryptoCoin = "USDT",
                    SoldCryptoCoin = "USDT",
                    ChangedAmountSoldCryptoCoin = createAccountRequest.TotalBalance
                };

                var cryptoCoins = await GetCryptoCoins.AsyncGetCryptoCoins();

                var usdt = cryptoCoins.Single(p => p.CoinName == "USDT");
                var tempBalance = new Balance
                {
                    CryptoCoinName = usdt.CoinName,
                    Account = account,
                    TotalBalance = createAccountRequest.TotalBalance
                };

                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _balanceRepository.AddAsync(tempBalance);
                await _unitOfWork.CommitAsync();
                logMessages = await _commonFunctions.GetLogResponseMessage("DepositFundsSuccess", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Success(201);
            }
            else
            {
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = account,
                    MessageForChanging = createAccountRequest.TotalBalance + " USDT deposit into the account",
                    ChangedAmount = createAccountRequest.TotalBalance,
                    BoughtCryptoCoin = "USDT",
                    SoldCryptoCoin = "USDT",
                    ChangedAmountSoldCryptoCoin = createAccountRequest.TotalBalance
                };
                balance.TotalBalance += createAccountRequest.TotalBalance;
                // tempBalance.CryptoCoinId = 3;
                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _unitOfWork.CommitAsync();
                logMessages = await _commonFunctions.GetLogResponseMessage("DepositFundsSuccess", language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Success(201);
            }
        }
    }
}
