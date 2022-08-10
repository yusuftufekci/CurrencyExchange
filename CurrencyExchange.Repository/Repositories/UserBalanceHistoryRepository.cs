using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Repositories;

namespace CurrencyExchange.Repository.Repositories
{
    public class UserBalanceHistoryRepository : GenericRepository<UserBalanceHistory>, IUserBalanceHistoryRepository
    {
        public UserBalanceHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
