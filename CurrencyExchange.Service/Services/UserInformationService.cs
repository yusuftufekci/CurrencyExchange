﻿using CurrencyExchange.Core.DTOs;
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
    public class UserInformationService<T> : IUserInformationService<T> where T : class
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IUnitOfWork _UnitOfWork;

        public UserInformationService(IUserRepository userRepository, IAccountRepository accountRepository, IUserBalanceHistoryRepository userBalanceHistoryRepository, IBalanceRepository balanceRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _UnitOfWork = unitOfWork;
        }

   

        public async Task<CustomResponseDto<UserInformationDto>> GetUserInformation(UserInformationRequest userInformationRequest)
        {
            var userExist = await _userRepository.Where(p => p.UserEmail == userInformationRequest.UserEmail).SingleOrDefaultAsync();


            var accountExist = await _accountRepository.Where(p => p.UserId == userExist.Id).SingleOrDefaultAsync();

            var balances = await _balanceRepository.Where(p => p.Account == accountExist).ToListAsync();

            List<BalanceDto> userBalancesInfos = new List<BalanceDto>();

            foreach (var item in balances)
            {
                BalanceDto userBalancesInfo = new BalanceDto();
                userBalancesInfo.TotalAmount = item.TotalBalance;
                userBalancesInfo.CryptoCoinName = item.CryptoCoin.CoinName;
                userBalancesInfos.Add(userBalancesInfo);
            }
            UserInformationDto userInformations = new UserInformationDto();
            userInformations.UserBalances = userBalancesInfos;
            userInformations.UserAccountName = accountExist.AccountName;
            userInformations.UserEmail = userInformationRequest.UserEmail;

            return CustomResponseDto<UserInformationDto>.Succes(201, userInformations);

        }

        public async Task<CustomResponseDto<List<UserTransactionHistoryDto>>> GetUserTranstactions(UserInformationRequest userInformationRequest)
        {
            var userExist = await _userRepository.Where(p => p.UserEmail == userInformationRequest.UserEmail).SingleOrDefaultAsync();

            var accountExist = await _accountRepository.Where(p => p.UserId == userExist.Id).SingleOrDefaultAsync();

            List<UserTransactionHistoryDto> userTransactionHistories = new List<UserTransactionHistoryDto>();
            var transactions = await _userBalanceHistoryRepository.Where(p => p.Account == accountExist).ToListAsync();
            foreach (var item in transactions)
            {
                UserTransactionHistoryDto userTransactions = new UserTransactionHistoryDto();
                userTransactions.MessageForChanging = item.MessageForChanging;
                userTransactions.AccountName = accountExist.AccountName;
                userTransactions.ChangedAmount = item.ChangedAmount;
                userTransactions.ExchangedCoinName = item.ExchangedCoinName;

                userTransactionHistories.Add(userTransactions);
            }

            return CustomResponseDto<List<UserTransactionHistoryDto>>.Succes(201, userTransactionHistories);
        }

        public async Task<CustomResponseDto<List<BalanceDto>>> GetUserBalanceInformation(UserInformationRequest userInformationRequest)
        {
            var userExist = await _userRepository.Where(p => p.UserEmail == userInformationRequest.UserEmail).SingleOrDefaultAsync();

            var accountExist = await _accountRepository.Where(p => p.UserId == userExist.Id).SingleOrDefaultAsync();

            var balances = await _balanceRepository.Where(p => p.Account == accountExist).ToListAsync();

            List<BalanceDto> userBalancesInfos = new List<BalanceDto>();

            foreach (var item in balances)
            {
                BalanceDto userBalancesInfo = new BalanceDto();
                userBalancesInfo.TotalAmount = item.TotalBalance;
                userBalancesInfo.CryptoCoinName = item.CryptoCoin.CoinName;
                userBalancesInfos.Add(userBalancesInfo);

            }
            return CustomResponseDto<List<BalanceDto>>.Succes(201, userBalancesInfos);
        }
    }
}
