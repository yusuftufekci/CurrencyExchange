using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Account;
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
    public class SellCryptoCoinService<T> : ISellCryptoCoinService<T> where T : class
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ICryptoCoinPriceRepository _cryptoCoinPriceRepository;
        private readonly ICryptoCoinRepository _cryptoCoinRepository;
        private readonly ISenderLogger _sender;

        public SellCryptoCoinService(IUserRepository repository, IUnitOfWork unitOfWork, IAccountRepository accountRepository,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
           IBalanceRepository balanceRepository,
           ICryptoCoinPriceRepository cryptoCoinPriceRepository, ICryptoCoinRepository cryptoCoinRepository, ISenderLogger sender)
        {
            _userRepository = repository;
            _UnitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _cryptoCoinPriceRepository = cryptoCoinPriceRepository;
            _cryptoCoinRepository = cryptoCoinRepository;
            _sender = sender;
        }

        public async Task<CustomResponseDto<NoContentDto>> SellCryptoCoin(SellCryptoCoinRequest sellCryptoCoinRequest)
        {
            /*
             Bu fonksiyon kullanıcı eğer belli bir miktar  Coinlik satış yapmak isterse kullanılacak.
            */

            var userExist = await _userRepository.Where(p => p.UserEmail == sellCryptoCoinRequest.UserEmail).SingleOrDefaultAsync();
            var accountExist = await _accountRepository.Where(p => p.User == userExist).SingleOrDefaultAsync();
            string symbolOfCoins = sellCryptoCoinRequest.CoinToSell + "USDT";
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin request failed. Account not found");
                return CustomResponseDto<NoContentDto>.Fail(404, "Account not found");
            }

            var coinTypeToBuy = await _cryptoCoinPriceRepository.Where(p => p.Symbol == symbolOfCoins).SingleOrDefaultAsync();
            if (coinTypeToBuy == null)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin request failed. There is no Crypto Coin name  " + sellCryptoCoinRequest.CoinToSell);
                return CustomResponseDto<NoContentDto>.Fail(404, "There is no Crypto Coin name  " + sellCryptoCoinRequest.CoinToSell);
            }
            var balanceExist = await _balanceRepository.Where(p => p.CryptoCoin.CoinName == sellCryptoCoinRequest.CoinToSell && p.Account == accountExist).SingleOrDefaultAsync();
            if (balanceExist == null)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin request failed. You dont have any" + sellCryptoCoinRequest.CoinToSell);
                return CustomResponseDto<NoContentDto>.Fail(404, "You dont have any" + sellCryptoCoinRequest.CoinToSell);
            }


            double coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            double totalAmount = coinPrice * sellCryptoCoinRequest.Amount; // Dolara ekleyeceğim miktar
            totalAmount = Math.Round(totalAmount, 4);
            if (totalAmount <= 0.001)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin request failed. User can't sell this amount of coin. Too low");
                return CustomResponseDto<NoContentDto>.Fail(404, "You can't sell this amount of coin. Too low");


            }
            if (sellCryptoCoinRequest.Amount > balanceExist.TotalBalance)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin request failed. User dont have enough " + sellCryptoCoinRequest.CoinToSell);
                return CustomResponseDto<NoContentDto>.Fail(404, "You dont have enough " + sellCryptoCoinRequest.CoinToSell);
            }

            UserBalanceHistory tempUserBalanceHistory = new UserBalanceHistory();
            var balanceExistForBuyCoin = await _balanceRepository.Where(p => p.CryptoCoin.CoinName == "USDT" && p.Account == accountExist).SingleOrDefaultAsync();

            balanceExistForBuyCoin.TotalBalance += totalAmount;
            balanceExist.TotalBalance -= sellCryptoCoinRequest.Amount;
            balanceExist.ModifiedDate = DateTime.UtcNow;

            tempUserBalanceHistory.Account = accountExist;
            tempUserBalanceHistory.MessageForChanging = sellCryptoCoinRequest.Amount + " " + sellCryptoCoinRequest.CoinToSell + " sold. It's equal to = " + totalAmount+" USDT";
            tempUserBalanceHistory.ChangedAmount = sellCryptoCoinRequest.Amount;
            tempUserBalanceHistory.BoughtCryptoCoin = "USDT";
            tempUserBalanceHistory.SoldCryptoCoin = sellCryptoCoinRequest.CoinToSell;

            await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            await _UnitOfWork.CommitAsync();
            _sender.SenderFunction("Log", "SellCryptoCoin request succesfully completed");

            return CustomResponseDto<NoContentDto>.Succes(201);

        }

        public async Task<CustomResponseDto<NoContentDto>> SellCryptoCoinV2(SellCryptoCoinRequest sellCryptoCoinRequest)
            /*
             Bu fonksiyon kullanıcı eğer belli bir miktar dolarlık satış yapmak isterse kullanılacak.
            */ 
            
        {

            var userExist = await _userRepository.Where(p => p.UserEmail == sellCryptoCoinRequest.UserEmail).SingleOrDefaultAsync();
            var accountExist = await _accountRepository.Where(p => p.User == userExist).SingleOrDefaultAsync();
            string symbolOfCoins = sellCryptoCoinRequest.CoinToSell + "USDT";
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin2 request failed. Account not found");
                return CustomResponseDto<NoContentDto>.Fail(404, "Account not found");

            }

            var coinTypeToBuy = await _cryptoCoinPriceRepository.Where(p => p.Symbol == symbolOfCoins).SingleOrDefaultAsync();
            if (coinTypeToBuy == null)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin2 request failed. There is no Crypto Coin name  " + sellCryptoCoinRequest.CoinToSell);
                return CustomResponseDto<NoContentDto>.Fail(404, "There is no Crypto Coin name  " + sellCryptoCoinRequest.CoinToSell);
            }
            var balanceExist = await _balanceRepository.Where(p => p.CryptoCoin.CoinName == sellCryptoCoinRequest.CoinToSell && p.Account == accountExist).SingleOrDefaultAsync();
            if (balanceExist == null)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin2 request failed. You dont have any" + sellCryptoCoinRequest.CoinToSell);
                return CustomResponseDto<NoContentDto>.Fail(404, "You dont have any" + sellCryptoCoinRequest.CoinToSell);
            }

            double coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            double totalAmount = sellCryptoCoinRequest.Amount /coinPrice ; //Coinden  çıkaracağım miktar
            totalAmount = Math.Round(totalAmount, 4);
            if (totalAmount <= 0.001)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin2 request failed. User can't sell this amount of coin. Too low");
                return CustomResponseDto<NoContentDto>.Fail(404, "You can't sell this amount of coin. Too low");

            }
            if (totalAmount > balanceExist.TotalBalance)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin2 request failed. User dont have enough " + sellCryptoCoinRequest.CoinToSell);
                return CustomResponseDto<NoContentDto>.Fail(404, "You dont have enough " + sellCryptoCoinRequest.CoinToSell);
            }

            UserBalanceHistory tempUserBalanceHistory = new UserBalanceHistory();
            var balanceExistForBuyCoin = await _balanceRepository.Where(p => p.CryptoCoin.CoinName == "USDT" && p.Account == accountExist).SingleOrDefaultAsync();

            balanceExistForBuyCoin.TotalBalance += sellCryptoCoinRequest.Amount;
            balanceExist.TotalBalance -= totalAmount;
            balanceExist.ModifiedDate = DateTime.UtcNow;

            tempUserBalanceHistory.Account = accountExist;
            tempUserBalanceHistory.MessageForChanging = totalAmount + " " + sellCryptoCoinRequest.CoinToSell + " sold. It's equal to = " + sellCryptoCoinRequest.Amount + " USDT";
            tempUserBalanceHistory.ChangedAmount = sellCryptoCoinRequest.Amount;
            tempUserBalanceHistory.BoughtCryptoCoin = "USDT";
            tempUserBalanceHistory.SoldCryptoCoin = sellCryptoCoinRequest.CoinToSell;

            await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            await _UnitOfWork.CommitAsync();
            _sender.SenderFunction("Log", "SellCryptoCoin2 request succesfully completed");

            return CustomResponseDto<NoContentDto>.Succes(201);


        }
    }
}
