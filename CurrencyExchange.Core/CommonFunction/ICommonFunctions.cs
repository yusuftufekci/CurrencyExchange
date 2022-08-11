using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.Authentication;

namespace CurrencyExchange.Core.CommonFunction
{
    public interface ICommonFunctions
    {
        Task<Account> GetAccount(string token);
        Task<User> GetUser(string token);



    }
}
