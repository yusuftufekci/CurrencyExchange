using CurrencyExchange.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.Authentication;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Service.CommonFunction
{
    public class CommonFunctions : ICommonFunctions
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserBalanceHistoryRepository _userBalanceHistoryRepository;
        public CommonFunctions(ITokenRepository tokenRepository , IUserRepository userRepository, IAccountRepository accountRepository, IUserBalanceHistoryRepository userBalanceHistoryRepository)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _userBalanceHistoryRepository = userBalanceHistoryRepository;
        }
        public async Task<Account> GetAccount(string token)
        {
            var tokenExists = await _tokenRepository.Where(p => p.Token == token).SingleAsync();
            var userExist = await _userRepository.Where(p => p.Id == tokenExists.UserId).SingleAsync();
            var accountExist = await _accountRepository.Where(p => p.User == userExist).SingleOrDefaultAsync();
            return accountExist ?? null;
        }

        public async Task<User> GetUser(string token)
        {
            var tokenExists = await _tokenRepository.Where(p => p.Token == token).SingleAsync();
            var userExist = await _userRepository.Where(p => p.Id == tokenExists.UserId).SingleAsync();
            return userExist ?? null;

        }
    }
}
