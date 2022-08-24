using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Entities.Log;
using CurrencyExchange.Core.Entities.LogMessages;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Service.CommonFunction
{
    public class CommonFunctions : ICommonFunctions
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IResponseMessageRepository _responseMessageRepository;
        private readonly ILogMessagesRepository _logMessagesRepository;

        public CommonFunctions(ITokenRepository tokenRepository, IUserRepository userRepository,
            IAccountRepository accountRepository, IResponseMessageRepository responseMessageRepository,
            ILogMessagesRepository logMessagesRepository)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _responseMessageRepository = responseMessageRepository;
            _logMessagesRepository = logMessagesRepository;
        }

        public async Task<Account> GetAccount(string token)
        {
            var userToken = await _tokenRepository.Where(p => p.Token == token).SingleAsync();
            var user = await _userRepository.Where(p => p.Id == userToken.UserId).SingleAsync();
            var account = await _accountRepository.Where(p => p.User == user).SingleOrDefaultAsync();
            return account ?? null;
        }

        public async Task<User> GetUser(string token)
        {
            var userToken = await _tokenRepository.Where(p => p.Token == token).SingleAsync();
            var user = await _userRepository.Where(p => p.Id == userToken.UserId).SingleAsync();
            return user ?? null;
        }

        public async Task<ResponseMessages> GetApiResponseMessage(string key, string language)
        {
            var responseMessage = await _responseMessageRepository.Where(p => p.Key == key && p.Language == language)
                .SingleAsync();
            return responseMessage ?? null;
        }

        public async Task<LogMessages> GetLogResponseMessage(string key, string language)
        {
            var responseMessage = await _logMessagesRepository.Where(p => p.Key == key && p.Language == language)
                .SingleAsync();
            return responseMessage ?? null;
        }
    }
}