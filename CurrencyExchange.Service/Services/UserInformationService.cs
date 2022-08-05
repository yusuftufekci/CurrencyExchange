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
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly ICryptoCoinRepository _cryptoCoinRepository;
        private readonly ISenderLogger _sender;

        public UserInformationService(IUserRepository userRepository, IAccountRepository accountRepository,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
            IBalanceRepository balanceRepository, IUnitOfWork unitOfWork, ICryptoCoinRepository cryptoCoinRepository, ISenderLogger sender)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _UnitOfWork = unitOfWork;
            _cryptoCoinRepository = cryptoCoinRepository;
            _sender = sender;
        }



        public async Task<CustomResponseDto<UserInformationDto>> GetUserInformation(UserInformationRequest userInformationRequest)
        {
            var accountExist = await _accountRepository.Where(p => p.User.UserEmail == userInformationRequest.UserEmail).SingleOrDefaultAsync();
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "CreateAccount request failed. Account not found");
                return CustomResponseDto<UserInformationDto>.Fail(404, "Account not found");
            }

            var balances = await _balanceRepository.Where(p => p.Account == accountExist).ToListAsync();

            List<BalanceDto> userBalancesInfos = new List<BalanceDto>();

            foreach (var item in balances)
            {
                BalanceDto userBalancesInfo = new BalanceDto();
                userBalancesInfo.TotalAmount = item.TotalBalance;
                var tempCoin = await _cryptoCoinRepository.Where(p => p.Id == item.CryptoCoinId).SingleOrDefaultAsync();

                userBalancesInfo.CryptoCoinName = tempCoin.CoinName;
                userBalancesInfos.Add(userBalancesInfo);
            }
            UserInformationDto userInformations = new UserInformationDto();
            userInformations.UserBalances = userBalancesInfos;
            userInformations.UserAccountName = accountExist.AccountName;
            userInformations.UserEmail = userInformationRequest.UserEmail;
            _sender.SenderFunction("Log", "GetUserInformation request succesfully completed");

            return CustomResponseDto<UserInformationDto>.Succes(201, userInformations);

        }

        public async Task<CustomResponseDto<List<UserTransactionHistoryDto>>> GetUserTranstactions(UserInformationRequest userInformationRequest)
        {
            var accountExist = await _accountRepository.Where(p => p.User.UserEmail == userInformationRequest.UserEmail).SingleOrDefaultAsync();
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "GetUserTranstactions request failed. Account not found");
                return CustomResponseDto<List<UserTransactionHistoryDto>>.Fail(404, "Account not found");

            }

            List<UserTransactionHistoryDto> userTransactionHistories = new List<UserTransactionHistoryDto>();
            var transactions = await _userBalanceHistoryRepository.Where(p => p.Account == accountExist).ToListAsync();
            foreach (var item in transactions)
            {
                UserTransactionHistoryDto userTransactions = new UserTransactionHistoryDto();
                userTransactions.MessageForChanging = item.MessageForChanging;
                userTransactions.AccountName = accountExist.AccountName;
                userTransactions.ChangedAmount = item.ChangedAmount;
                userTransactions.BoughtCryptoCoin = item.BoughtCryptoCoin;
                userTransactions.SoldCryptoCoin = item.SoldCryptoCoin;

                userTransactionHistories.Add(userTransactions);
            }
            _sender.SenderFunction("Log", "GetUserTranstactions request succesfully completed");

            return CustomResponseDto<List<UserTransactionHistoryDto>>.Succes(201, userTransactionHistories);
        }

        public async Task<CustomResponseDto<List<BalanceDto>>> GetUserBalanceInformation(UserInformationRequest userInformationRequest)
        {
            var accountExist = await _accountRepository.Where(p => p.User.UserEmail == userInformationRequest.UserEmail).SingleOrDefaultAsync();
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "GetUserBalanceInformation request failed. Account not found");
                return CustomResponseDto<List<BalanceDto>>.Fail(404, "Account not found");

            }

            var balances = await _balanceRepository.Where(p => p.Account == accountExist).ToListAsync();

            List<BalanceDto> userBalancesInfos = new List<BalanceDto>();

            foreach (var item in balances)
            {
                BalanceDto userBalancesInfo = new BalanceDto();
                userBalancesInfo.TotalAmount = item.TotalBalance;
                var tempCoin = await _cryptoCoinRepository.Where(p => p.Id == item.CryptoCoinId).SingleOrDefaultAsync();
                userBalancesInfo.CryptoCoinName = tempCoin.CoinName;
                userBalancesInfos.Add(userBalancesInfo);

            }
            _sender.SenderFunction("Log", "GetUserBalanceInformation request succesfully completed");

            return CustomResponseDto<List<BalanceDto>>.Succes(201, userBalancesInfos);
        }
    }
}
