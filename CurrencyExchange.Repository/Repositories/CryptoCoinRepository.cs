using CurrencyExchange.Core.Entities.CryptoCoins;
using CurrencyExchange.Core.Repositories;

namespace CurrencyExchange.Repository.Repositories
{
    public class CryptoCoinRepository : GenericRepository<CryptoCoin>, ICryptoCoinRepository
    {
        public CryptoCoinRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
