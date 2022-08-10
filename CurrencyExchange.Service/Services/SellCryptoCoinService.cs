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
    public class SellCryptoCoinService<T> : ISellCryptoCoinService<T> where T : class
    {
        private  readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ISenderLogger _sender;

        public SellCryptoCoinService(IUserRepository repository, IUnitOfWork unitOfWork, IAccountRepository accountRepository,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
           IBalanceRepository balanceRepository, 
            ISenderLogger sender, ITokenRepository tokenRepository)
        {
            _userRepository = repository;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _sender = sender;
            _tokenRepository = tokenRepository;
        }

        public async Task<CustomResponseDto<NoContentDto>> SellCryptoCoin(SellCryptoCoinRequest sellCryptoCoinRequest, string token)
        {
            /*
             Bu fonksiyon kullanıcı eğer belli bir miktar  Coinlik satış yapmak isterse kullanılacak.
            */
            var tokenExists = await _tokenRepository.Where(p => p.Token == token).SingleOrDefaultAsync();
            var userExist = await _userRepository.Where(p => p.Id == tokenExists.UserId).SingleOrDefaultAsync(); var accountExist = await _accountRepository.Where(p => p.User == userExist).SingleOrDefaultAsync();
            var symbolOfCoins = sellCryptoCoinRequest.CoinToSell + "USDT";
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin request failed. Account not found");
                return CustomResponseDto<NoContentDto>.Fail(404, "Account not found");
            }
            var cryptoCoinPrices = await GetCryptoCoinPrices.AsyncGetCryptoCoinPrices();
            var coinTypeToBuy = cryptoCoinPrices.SingleOrDefault(p => p.Symbol == symbolOfCoins); if (coinTypeToBuy == null)
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
            var coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            var totalAmount = coinPrice * sellCryptoCoinRequest.Amount;
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
            var balanceExistForBuyCoin = await _balanceRepository.Where(p => p.CryptoCoin.CoinName == "USDT" && p.Account == accountExist).SingleOrDefaultAsync();
            balanceExistForBuyCoin.TotalBalance += totalAmount;
            balanceExist.TotalBalance -= sellCryptoCoinRequest.Amount;
            balanceExist.ModifiedDate = DateTime.UtcNow;
            var tempUserBalanceHistory = new UserBalanceHistory
            {
                Account = accountExist,
                MessageForChanging = sellCryptoCoinRequest.Amount + " " + sellCryptoCoinRequest.CoinToSell + " sold. It's equal to = " + totalAmount + " USDT",
                ChangedAmount = sellCryptoCoinRequest.Amount,
                BoughtCryptoCoin = "USDT",
                SoldCryptoCoin = sellCryptoCoinRequest.CoinToSell
            };
            await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            await _unitOfWork.CommitAsync();
            _sender.SenderFunction("Log", "SellCryptoCoin request succesfully completed");
            return CustomResponseDto<NoContentDto>.Succes(201);

        }

        public async Task<CustomResponseDto<NoContentDto>> SellCryptoCoinV2(SellCryptoCoinRequest sellCryptoCoinRequest, string token)
        {
            /*
             Bu fonksiyon kullanıcı eğer belli bir miktar dolarlık satış yapmak isterse kullanılacak.
            */

            var tokenExists = await _tokenRepository.Where(p => p.Token == token).SingleOrDefaultAsync();
            var userExist = await _userRepository.Where(p => p.Id == tokenExists.UserId).SingleOrDefaultAsync();
            var accountExist = await _accountRepository.Where(p => p.User == userExist).SingleOrDefaultAsync();
            var symbolOfCoins = sellCryptoCoinRequest.CoinToSell + "USDT";
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "SellCryptoCoin2 request failed. Account not found");
                return CustomResponseDto<NoContentDto>.Fail(404, "Account not found");
            }
            var cryptoCoinPrices = await GetCryptoCoinPrices.AsyncGetCryptoCoinPrices();
            var coinTypeToBuy = cryptoCoinPrices.SingleOrDefault(p => p.Symbol == symbolOfCoins); if (coinTypeToBuy == null)
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
            var coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            var totalAmount = sellCryptoCoinRequest.Amount /coinPrice ; //Coinden  çıkaracağım miktar
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
            var balanceExistForBuyCoin = await _balanceRepository.Where(p => p.CryptoCoin.CoinName == "USDT" && p.Account == accountExist).SingleOrDefaultAsync();
            balanceExistForBuyCoin.TotalBalance += sellCryptoCoinRequest.Amount;
            balanceExist.TotalBalance -= totalAmount;
            balanceExist.ModifiedDate = DateTime.UtcNow;
            var tempUserBalanceHistory = new UserBalanceHistory
            {
                Account = accountExist,
                MessageForChanging = totalAmount + " " + sellCryptoCoinRequest.CoinToSell + " sold. It's equal to = " + sellCryptoCoinRequest.Amount + " USDT",
                ChangedAmount = sellCryptoCoinRequest.Amount,
                BoughtCryptoCoin = "USDT",
                SoldCryptoCoin = sellCryptoCoinRequest.CoinToSell
            };
            await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            await _unitOfWork.CommitAsync();
            _sender.SenderFunction("Log", "SellCryptoCoin2 request successfully completed");
            return CustomResponseDto<NoContentDto>.Succes(201);
        }
    }
}
