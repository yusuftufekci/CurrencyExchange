using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Core.HelperFunctions;

namespace CurrencyExchange.Service.Services
{
    public class BuyCryptoCoinService<T> : IBuyCryptoCoinService<T> where T : class
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ICryptoCoinRepository _cryptoCoinRepository;
        private readonly ISenderLogger _sender;

        public BuyCryptoCoinService(IUserRepository repository, IUnitOfWork unitOfWork, IAccountRepository accountRepository,
            IUserBalanceHistoryRepository userBalanceHistoryRepository,
           IBalanceRepository balanceRepository, 
            ICryptoCoinRepository cryptoCoinRepository, ISenderLogger sender, ITokenRepository tokenRepository)
        {
            _userRepository = repository;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
            _balanceRepository = balanceRepository;
            _cryptoCoinRepository = cryptoCoinRepository;
            _sender = sender;
            _tokenRepository = tokenRepository;
        }

        public async Task<CustomResponseDto<NoContentDto>> BuyCoinWithAmount(BuyCoinRequest buyCoinRequest, string token)
        {
            var tokenExists = await _tokenRepository.Where(p => p.Token == token).SingleOrDefaultAsync();
            var userExist = await _userRepository.Where(p => p.Id == tokenExists.UserId).SingleOrDefaultAsync();
            var accountExist = await _accountRepository.Where(p => p.User == userExist).SingleOrDefaultAsync();
            var symbolOfCoins = buyCoinRequest.CoinToBuy + buyCoinRequest.BuyWIthThisCoin;
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "BuyCoinWithAmount request failed. Account not found");
                return CustomResponseDto<NoContentDto>.Fail(404,"Account not found");
            }
            var balanceExist = await _balanceRepository.Where(p => p.CryptoCoin.CoinName == buyCoinRequest.BuyWIthThisCoin && p.Account == accountExist).SingleOrDefaultAsync();
            if (balanceExist == null)
            {
                _sender.SenderFunction("Log", "BuyCoinWithAmount request failed. User don't have any" + buyCoinRequest.BuyWIthThisCoin);
                return CustomResponseDto<NoContentDto>.Fail(404, $"You don't have any" + buyCoinRequest.BuyWIthThisCoin);
            }
            var cryptoCoinPrices = await GetCryptoCoinPrices.AsyncGetCryptoCoinPrices();
            var coinTypeToBuy = cryptoCoinPrices.SingleOrDefault(p => p.Symbol == symbolOfCoins);
            if (coinTypeToBuy == null)
            {
                _sender.SenderFunction("Log", "BuyCoinWithAmount request failed. User can't buy " + buyCoinRequest.CoinToBuy + " with " + buyCoinRequest.BuyWIthThisCoin);
                return CustomResponseDto<NoContentDto>.Fail(404, "You can't buy " + buyCoinRequest.CoinToBuy + " with " + buyCoinRequest.BuyWIthThisCoin);
            }
            var coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            var totalAmount = coinPrice * buyCoinRequest.Amount;
            totalAmount = Math.Round(totalAmount, 4);
            if (totalAmount <= 0.001)
            {
                _sender.SenderFunction("Log", "BuyCoinWithAmount request failed. User can't buy this amount of coin");
                return CustomResponseDto<NoContentDto>.Fail(404, "You can't buy this amount of coin");
            }
            if (totalAmount > balanceExist.TotalBalance)
            {
                _sender.SenderFunction("Log", "BuyCoinWithAmount request failed. User don't have enough" + buyCoinRequest.BuyWIthThisCoin);
                return CustomResponseDto<NoContentDto>.Fail(404, "You don't have enough" + buyCoinRequest.BuyWIthThisCoin);
            }
            var balanceExistForBuyCoin = await _balanceRepository.Where(p => p.Account == accountExist && p.CryptoCoin.CoinName == buyCoinRequest.CoinToBuy).SingleOrDefaultAsync();
            var coinToBuy = await _cryptoCoinRepository.Where(p => p.CoinName == buyCoinRequest.CoinToBuy).SingleOrDefaultAsync();
            if (balanceExistForBuyCoin == null)
            {
                var tempBalance = new Balance
                {
                    CryptoCoin = coinToBuy,
                    Account = accountExist,
                    TotalBalance = buyCoinRequest.Amount
                 };
                balanceExist.TotalBalance -= totalAmount;
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = accountExist,
                    MessageForChanging = buyCoinRequest.Amount + " " + buyCoinRequest.CoinToBuy + " deposit into the account",
                    ChangedAmount = buyCoinRequest.Amount,
                    BoughtCryptoCoin = buyCoinRequest.CoinToBuy,
                    SoldCryptoCoin = buyCoinRequest.BuyWIthThisCoin,
                    ChangedAmountSoldCryptoCoin = totalAmount
                 };
                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _balanceRepository.AddAsync(tempBalance);
            }
            else
            {
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = accountExist,
                    MessageForChanging = buyCoinRequest.Amount + " " + buyCoinRequest.CoinToBuy + " deposit into the account",
                    ChangedAmount = buyCoinRequest.Amount,
                    BoughtCryptoCoin = buyCoinRequest.CoinToBuy,
                    SoldCryptoCoin = buyCoinRequest.BuyWIthThisCoin,
                    ChangedAmountSoldCryptoCoin = totalAmount
                };
                balanceExistForBuyCoin.TotalBalance += buyCoinRequest.Amount;
                balanceExist.TotalBalance -= totalAmount;
                balanceExist.ModifiedDate = DateTime.UtcNow;
                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            }
            await _unitOfWork.CommitAsync();
            _sender.SenderFunction("Log", "BuyCoinWithAmount request succesfully completed.");
            return CustomResponseDto<NoContentDto>.Succes(201);

        }

        

        public async Task<CustomResponseDto<NoContentDto>> BuyCoinWithAmount2(BuyCoinRequest buyCoinRequest, string token)
        {
            var tokenExists = await _tokenRepository.Where(p => p.Token == token).SingleOrDefaultAsync();
            var userExist = await _userRepository.Where(p => p.Id == tokenExists.UserId).SingleOrDefaultAsync(); var accountExist = await _accountRepository.Where(p => p.User == userExist).SingleOrDefaultAsync();
            var symbolOfCoins = buyCoinRequest.CoinToBuy + buyCoinRequest.BuyWIthThisCoin;
            if (accountExist == null)
            {
                _sender.SenderFunction("Log", "BuyCoinWithAmount2 request failed. Account not found");
                return CustomResponseDto<NoContentDto>.Fail(404, "Account not found");
            }
            var balanceExist = await _balanceRepository.Where(p => p.CryptoCoin.CoinName == buyCoinRequest.BuyWIthThisCoin && p.Account == accountExist).SingleOrDefaultAsync();
            if (balanceExist == null)
            {
                _sender.SenderFunction("Log", "BuyCoinWithAmount2 request failed. User dont have any" + buyCoinRequest.BuyWIthThisCoin);
                return CustomResponseDto<NoContentDto>.Fail(404, $"You dont have any" + buyCoinRequest.BuyWIthThisCoin);
            }
            var cryptoCoinPrices = await GetCryptoCoinPrices.AsyncGetCryptoCoinPrices();
            var coinTypeToBuy = cryptoCoinPrices.SingleOrDefault(p => p.Symbol == symbolOfCoins); if (coinTypeToBuy == null)
            {
                _sender.SenderFunction("Log", "BuyCoinWithAmount2 request failed. User can't buy " + buyCoinRequest.CoinToBuy + " with " + buyCoinRequest.BuyWIthThisCoin);
                return CustomResponseDto<NoContentDto>.Fail(404, "You can't buy " + buyCoinRequest.CoinToBuy + " with " + buyCoinRequest.BuyWIthThisCoin);
            }
            var coinPrice = Convert.ToDouble(coinTypeToBuy.Price);
            var totalAmount = buyCoinRequest.Amount / coinPrice;
            totalAmount = Math.Round(totalAmount, 4);
            if (totalAmount <= 0.001)
            {
                _sender.SenderFunction("Log", "BuyCoinWithAmount2 request failed. User can't buy this amount of coin");
                return CustomResponseDto<NoContentDto>.Fail(404, "You can't buy this amount of coin");
            }

            if (totalAmount > balanceExist.TotalBalance)
            {
                _sender.SenderFunction("Log", "BuyCoinWithAmount2 request failed. User dont have enough" + buyCoinRequest.BuyWIthThisCoin);
                return CustomResponseDto<NoContentDto>.Fail(404, "You dont have enough" + buyCoinRequest.BuyWIthThisCoin);
            }
            var balanceExistForBuyCoin = await _balanceRepository.Where(p => p.Account == accountExist && p.CryptoCoin.CoinName == buyCoinRequest.CoinToBuy).SingleOrDefaultAsync();
            var coinToBuy = await _cryptoCoinRepository.Where(p => p.CoinName == buyCoinRequest.CoinToBuy).SingleOrDefaultAsync();
            if (balanceExistForBuyCoin == null)
            {
                var tempBalance = new Balance
                {
                    CryptoCoin = coinToBuy,
                    Account = accountExist,
                    TotalBalance = totalAmount
                 };
                balanceExist.TotalBalance -= buyCoinRequest.Amount;
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = accountExist,
                    MessageForChanging = totalAmount + " " + buyCoinRequest.CoinToBuy + " deposit into the account",
                    ChangedAmount = totalAmount,
                    BoughtCryptoCoin = buyCoinRequest.CoinToBuy,
                    SoldCryptoCoin = buyCoinRequest.BuyWIthThisCoin,
                    ChangedAmountSoldCryptoCoin = buyCoinRequest.Amount
                };
                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
                await _balanceRepository.AddAsync(tempBalance);
            }
            else
            {
                balanceExistForBuyCoin.TotalBalance += totalAmount;
                balanceExist.TotalBalance -= buyCoinRequest.Amount;
                balanceExist.ModifiedDate = DateTime.UtcNow;
                var tempUserBalanceHistory = new UserBalanceHistory
                {
                    Account = accountExist,
                    MessageForChanging = totalAmount + " " + buyCoinRequest.CoinToBuy + " deposit into the account",
                    ChangedAmount = totalAmount,
                    BoughtCryptoCoin = buyCoinRequest.CoinToBuy,
                    SoldCryptoCoin = buyCoinRequest.BuyWIthThisCoin,
                    ChangedAmountSoldCryptoCoin = buyCoinRequest.Amount
                 };
                await _userBalanceHistoryRepository.AddAsync(tempUserBalanceHistory);
            }
            await _unitOfWork.CommitAsync();
            _sender.SenderFunction("Log", "BuyCoinWithAmount2 request successfully completed.");
            return CustomResponseDto<NoContentDto>.Succes(201);

        }
    }
}
