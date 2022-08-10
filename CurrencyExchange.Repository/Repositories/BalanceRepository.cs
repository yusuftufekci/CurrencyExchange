using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Repositories;

namespace CurrencyExchange.Repository.Repositories
{
    public class BalanceRepository : GenericRepository<Balance>, IBalanceRepository
    {
        public BalanceRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
