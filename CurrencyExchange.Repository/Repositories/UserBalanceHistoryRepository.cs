using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Repository.Repositories
{
    public class UserBalanceHistoryRepository : GenericRepository<UserBalanceHistory>, IUserBalanceHistoryRepository
    {
        public UserBalanceHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
