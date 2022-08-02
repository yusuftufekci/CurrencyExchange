using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Account;
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
    public class BuyCryptoCoinService<T> : IBuyCryptoCoinService<T> where T : class
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ICryptoCoinPriceRepository _cryptoCoinPriceRepository;
        private readonly ICryptoCoinRepository _cryptoCoinRepository;

        public BuyCryptoCoinService(IUserRepository repository, IUnitOfWork unitOfWork, IAccountRepository accountRepository,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
           IBalanceRepository balanceRepository,
           ICryptoCoinPriceRepository cryptoCoinPriceRepository, ICryptoCoinRepository cryptoCoinRepository)
        {
            _userRepository = repository;
            _UnitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _cryptoCoinPriceRepository = cryptoCoinPriceRepository;
            _cryptoCoinRepository = cryptoCoinRepository;

        }

        public async Task<CustomResponseDto<NoContentDto>> BuyCoinWithAmount(BuyCoinRequest buyCoinRequest)
        {
            var userExist = await _userRepository.Where(p => p.UserEmail == buyCoinRequest.UserEmail).SingleOrDefaultAsync();
            var accountExist = await _accountRepository.Where(p => p.User == userExist).SingleOrDefaultAsync();
            string symbolOfCoins = buyCoinRequest.CoinToBuy + buyCoinRequest.BuyWİthThisCoin;
            if (accountExist == null)
                throw new NotFoundException($"Account not found");

            var balanceExist = await _balanceRepository.Where(p => p.CryptoCoin.CoinName == buyCoinRequest.BuyWİthThisCoin && p.Account == accountExist).SingleOrDefaultAsync();
            if (balanceExist == null)
            {
                throw new NotFoundException($"You dont have any" + buyCoinRequest.BuyWİthThisCoin);
            }
            var coinTypeToBuy = await _cryptoCoinPriceRepository.Where(p => p.Symbol == symbolOfCoins).SingleOrDefaultAsync();
            if (coinTypeToBuy == null)
            {
                throw new NotFoundException($"You can't buy " + buyCoinRequest.CoinToBuy + " with " + buyCoinRequest.BuyWİthThisCoin);

            }
            double coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            double totalAmount = coinPrice * buyCoinRequest.Amount;
            totalAmount = Math.Round(totalAmount, 4);
            if (totalAmount <= 0.001)
                throw new ClientSideException("You can't buy this amount of coin");
            if (totalAmount > balanceExist.TotalBalance)
            {
                throw new ClientSideException("You dont have enough" + buyCoinRequest.BuyWİthThisCoin);
            }
            var balanceExistForBuyCoin = await _balanceRepository.Where(p => p.Account == accountExist && p.CryptoCoin.CoinName == buyCoinRequest.CoinToBuy).SingleOrDefaultAsync();
            var coinToBuy = await _cryptoCoinRepository.Where(p => p.CoinName == buyCoinRequest.CoinToBuy).SingleOrDefaultAsync();
            if (balanceExistForBuyCoin == null)
            {
                Balance tempBalance = new Balance();
                UserBalanceHistory tempUserBalanceHistory = new UserBalanceHistory();


                balanceExist.TotalBalance -= totalAmount;
                tempBalance.CryptoCoin = coinToBuy;
                tempBalance.Account = accountExist;
                tempBalance.TotalBalance = buyCoinRequest.Amount;

                tempUserBalanceHistory.Account = accountExist;
                tempUserBalanceHistory.MessageForChanging = buyCoinRequest.Amount + " " + buyCoinRequest.CoinToBuy + " deposit into the account";
                tempUserBalanceHistory.ChangedAmount = buyCoinRequest.Amount;
                tempUserBalanceHistory.ExchangedCoinName = buyCoinRequest.CoinToBuy;

                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _balanceRepository.AddAsync(tempBalance);
                await _UnitOfWork.CommitAsync();
                
            }
            else
            {
                UserBalanceHistory tempUserBalanceHistory = new UserBalanceHistory();

                balanceExistForBuyCoin.TotalBalance += buyCoinRequest.Amount;
                balanceExist.TotalBalance -= totalAmount;
                balanceExist.ModifiedDate = DateTime.UtcNow;

                tempUserBalanceHistory.Account = accountExist;
                tempUserBalanceHistory.MessageForChanging = buyCoinRequest.Amount + " " + buyCoinRequest.CoinToBuy + " deposit into the account";
                tempUserBalanceHistory.ChangedAmount = buyCoinRequest.Amount;
                tempUserBalanceHistory.ExchangedCoinName = buyCoinRequest.CoinToBuy;

                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _UnitOfWork.CommitAsync();


            }

            return CustomResponseDto<NoContentDto>.Succes(201);

        }

        

        public async Task<CustomResponseDto<NoContentDto>> BuyCoinWithAmount2(BuyCoinRequest buyCoinRequest)
        {
            var userExist = await _userRepository.Where(p => p.UserEmail == buyCoinRequest.UserEmail).SingleOrDefaultAsync();
            var accountExist = await _accountRepository.Where(p => p.User == userExist).SingleOrDefaultAsync();
            string symbolOfCoins = buyCoinRequest.CoinToBuy + buyCoinRequest.BuyWİthThisCoin;
            if (accountExist == null)
                throw new NotFoundException($"Account not found");

            var balanceExist = await _balanceRepository.Where(p => p.CryptoCoin.CoinName == buyCoinRequest.BuyWİthThisCoin && p.Account == accountExist).SingleOrDefaultAsync();
            if (balanceExist == null)
            {
                throw new NotFoundException($"You dont have any" + buyCoinRequest.BuyWİthThisCoin);
            }
            var coinTypeToBuy = await _cryptoCoinPriceRepository.Where(p => p.Symbol == symbolOfCoins).SingleOrDefaultAsync();
            if (coinTypeToBuy == null)
            {
                throw new NotFoundException($"You can't buy " + buyCoinRequest.CoinToBuy + " with " + buyCoinRequest.BuyWİthThisCoin);

            }
            double coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            double totalAmount = buyCoinRequest.Amount / coinPrice;
            totalAmount = Math.Round(totalAmount, 4);

            if (totalAmount <= 0.001)
                throw new ClientSideException("You can't buy this amount of coin");

            if (totalAmount > balanceExist.TotalBalance)
            {
                throw new ClientSideException("You dont have enough" + buyCoinRequest.BuyWİthThisCoin);
            }
            var balanceExistForBuyCoin = await _balanceRepository.Where(p => p.Account == accountExist && p.CryptoCoin.CoinName == buyCoinRequest.CoinToBuy).SingleOrDefaultAsync();
            var coinToBuy = await _cryptoCoinRepository.Where(p => p.CoinName == buyCoinRequest.CoinToBuy).SingleOrDefaultAsync();

            if (balanceExistForBuyCoin == null)
            {
                Balance tempBalance = new Balance();
                UserBalanceHistory tempUserBalanceHistory = new UserBalanceHistory();


                balanceExist.TotalBalance -= buyCoinRequest.Amount;
                tempBalance.CryptoCoin = coinToBuy;
                tempBalance.Account = accountExist;
                tempBalance.TotalBalance = totalAmount;

                tempUserBalanceHistory.Account = accountExist;
                tempUserBalanceHistory.MessageForChanging = totalAmount + " " + buyCoinRequest.CoinToBuy + " deposit into the account";
                tempUserBalanceHistory.ChangedAmount = totalAmount;
                tempUserBalanceHistory.ExchangedCoinName = buyCoinRequest.CoinToBuy;


                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _balanceRepository.AddAsync(tempBalance);
                await _UnitOfWork.CommitAsync();


            }
            else
            {
                UserBalanceHistory tempUserBalanceHistory = new UserBalanceHistory();

                balanceExistForBuyCoin.TotalBalance += totalAmount;
                balanceExist.TotalBalance -= buyCoinRequest.Amount;
                balanceExist.ModifiedDate = DateTime.UtcNow;


                tempUserBalanceHistory.Account = accountExist;
                tempUserBalanceHistory.MessageForChanging = totalAmount + " " + buyCoinRequest.CoinToBuy + " deposit into the account";
                tempUserBalanceHistory.ChangedAmount = totalAmount;
                tempUserBalanceHistory.ExchangedCoinName = buyCoinRequest.CoinToBuy;
                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _UnitOfWork.CommitAsync();

            }
            return CustomResponseDto<NoContentDto>.Succes(201);

        }
    }
}
