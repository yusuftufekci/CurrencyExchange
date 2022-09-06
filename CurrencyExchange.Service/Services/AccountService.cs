using System.Net;
using CurrencyExchange.Caching.CryptoCoins;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.ConstantsMessages;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.LogMessages;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using CurrencyExchange.Service.LogFacade;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ISenderLogger _logSender;
        private readonly ICommonFunctions _commonFunctions;
        private readonly CryptoCoinServiceWithCaching _cryptoCoinServiceWithCaching;
        private readonly LogResponseFacade _logResponseFacade;



        public AccountService(IUnitOfWork unitOfWork, IAccountRepository accountRepository,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
            IBalanceRepository balanceRepository,
            ISenderLogger logSenderLogger , ICommonFunctions commonFunctions, CryptoCoinServiceWithCaching cryptoCoinServiceWithCaching, LogResponseFacade logResponseFacade)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _logSender = logSenderLogger;
            _commonFunctions = commonFunctions;
            _cryptoCoinServiceWithCaching = cryptoCoinServiceWithCaching;
            _logResponseFacade = logResponseFacade;
        }
        public async Task<CustomResponseDto<NoContentDto>> CreateAccount(CreateAccountRequest createAccountRequest, string token)
        {
            var user = await _commonFunctions.GetUser(token);
            var account = await _commonFunctions.GetAccount(token);

            if (account != null)
            {
                var responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.CreateAccountAccountAlreadyExist, ConstantResponseMessage.AccountAlreadyExist, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.Conflict, responseMessage.Value);
            }
            var tempAccount = new Account
            {
                AccountName = createAccountRequest.AccountName,
                UserId = user.Id
            };
            await _accountRepository.AddAsync(tempAccount);
            await _unitOfWork.CommitAsync();
            var logMessages = await _commonFunctions.GetLogResponseMessage(ConstantLogMessages.CreateAccountSuccess, language: "en");
            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<NoContentDto>.Success();
        }

        public async Task<CustomResponseDto<NoContentDto>> DepositFunds(DepositFundRequest createAccountRequest, string token)
        {
            LogMessages logMessages;
            var account = await _commonFunctions.GetAccount(token);
            if (account == null)
            {
                var responseMessage = await _logResponseFacade.GetLogAndResponseMessage(ConstantLogMessages.DepositFundsAccountNotFound, ConstantResponseMessage.AccountNotFound, "en");
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }

            var balance = await _balanceRepository.Where(p => p.Account == account && p.CryptoCoinName == Usdt.Name).SingleOrDefaultAsync();
            if (balance == null)
            {
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = account,
                    MessageForChanging = createAccountRequest.TotalBalance + " USDT deposit into the account",
                    ChangedAmount = createAccountRequest.TotalBalance,
                    BoughtCryptoCoin = Usdt.Name,
                    SoldCryptoCoin = Usdt.Name,
                    ChangedAmountSoldCryptoCoin = createAccountRequest.TotalBalance
                };

                var cryptoCoins = _cryptoCoinServiceWithCaching.GetCryptoCoins();

                var usdt = cryptoCoins.Single(p => p.CoinName == Usdt.Name);
                var tempBalance = new Balance
                {
                    CryptoCoinName = usdt.CoinName,
                    Account = account,
                    TotalBalance = createAccountRequest.TotalBalance
                };

                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _balanceRepository.AddAsync(tempBalance);
                await _unitOfWork.CommitAsync();
                logMessages = await _commonFunctions.GetLogResponseMessage(ConstantLogMessages.DepositFundsSuccess, language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Success();
            }
            else
            {
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = account,
                    MessageForChanging = createAccountRequest.TotalBalance + " USDT deposit into the account",
                    ChangedAmount = createAccountRequest.TotalBalance,
                    BoughtCryptoCoin = Usdt.Name,
                    SoldCryptoCoin = Usdt.Name,
                    ChangedAmountSoldCryptoCoin = createAccountRequest.TotalBalance
                };
                balance.TotalBalance += createAccountRequest.TotalBalance;
                // tempBalance.CryptoCoinId = 3;
                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _unitOfWork.CommitAsync();
                logMessages = await _commonFunctions.GetLogResponseMessage(ConstantLogMessages.DepositFundsSuccess, language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Success();
            }
        }
    }
}
