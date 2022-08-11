﻿using CurrencyExchange.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public CommonFunctions(ITokenRepository tokenRepository , IUserRepository userRepository, IAccountRepository accountRepository, IResponseMessageRepository responseMessageRepository, ILogMessagesRepository logMessagesRepository)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _responseMessageRepository = responseMessageRepository;
            _logMessagesRepository = logMessagesRepository;
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

        public async Task<ResponseMessages> GetApiResponseMessage(string key, string language)
        {
            var responseMessage = await _responseMessageRepository.Where(p => p.Key == key && p.Language == language).SingleAsync();
            return responseMessage ?? null;

        }

        public async Task<LogMessages> GetLogResponseMessage(string key, string language)
        {
            var responseMessage = await _logMessagesRepository.Where(p => p.Key == key && p.Language == language).SingleAsync();
            return responseMessage ?? null;

        }



    }
}
