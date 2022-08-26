using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.ConfigModels;
using CurrencyExchange.Core.DTOs.CryptoCoins;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Entities.CryptoCoins;
using CurrencyExchange.Core.Entities.Log;
using CurrencyExchange.Core.Entities.LogMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CurrencyExchange.Service.CommonFunction
{
    public class CommonFunctions : ICommonFunctions
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IResponseMessageRepository _responseMessageRepository;
        private readonly ILogMessagesRepository _logMessagesRepository;
        private readonly AppSettings _appSettings;
        private readonly UrlList _urlList;


        public CommonFunctions(ITokenRepository tokenRepository, IUserRepository userRepository,
            IAccountRepository accountRepository, IResponseMessageRepository responseMessageRepository,
            ILogMessagesRepository logMessagesRepository, AppSettings appSettings, IOptions<UrlList> urlList)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _responseMessageRepository = responseMessageRepository;
            _logMessagesRepository = logMessagesRepository;
            _appSettings = appSettings;
            _urlList = urlList.Value;
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

        public async Task<List<CryptoCoinPriceDto>?> GetCryptoCoinPrices()
        {
            var cryptoCoinPrices = new List<CryptoCoinPriceDto>();
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(1);
            var response = await client.GetAsync(_urlList.BinanceApiUrl);
            if (response.StatusCode != System.Net.HttpStatusCode.OK) return null;
            var responceString = await response.Content.ReadAsStringAsync();
            var responceObject = JsonConvert.DeserializeObject<List<CryptoCoinPriceDto>>(responceString);
            cryptoCoinPrices.AddRange(responceObject.Select(item =>
                new CryptoCoinPriceDto { Price = item.Price, Symbol = item.Symbol }));
            return cryptoCoinPrices;
        }

        public async Task<List<CryptoCoin>?> GetCryptoCoins()
        {
            var cryptoCoins = new List<CryptoCoin>();
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(1);
            var response =
                await client.GetAsync( _urlList.NomicsApiUrl);
            if (response.StatusCode != System.Net.HttpStatusCode.OK) return null;
            var responceString = await response.Content.ReadAsStringAsync();
            var root = (JContainer)JToken.Parse(responceString);
            var list = root.DescendantsAndSelf()
                .OfType<JProperty>()
                .Where(p => p.Name == "id")
                .Select(p => p.Value.Value<string>())
                .ToList();
            cryptoCoins.AddRange(list.Select(item => new CryptoCoin { CoinName = item }));

            return cryptoCoins;
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

        public string GenerateToken(User user)
        {
            var mySecret = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var mySecurityKey = new SymmetricSecurityKey((mySecret));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserEmail),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}