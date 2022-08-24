using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Entities.Log;
using CurrencyExchange.Core.Entities.LogMessages;

namespace CurrencyExchange.Core.CommonFunction
{
    public interface ICommonFunctions
    {
        Task<Account> GetAccount(string token);
        Task<User> GetUser(string token);

        Task<ResponseMessages> GetApiResponseMessage(string key, string language);
        public string GenerateToken(User user);

        Task<LogMessages> GetLogResponseMessage(string key, string language);
    }
}