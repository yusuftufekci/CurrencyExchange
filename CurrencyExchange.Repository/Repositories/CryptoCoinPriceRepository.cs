using CurrencyExchange.Core.Entities.CryptoCoins;
using CurrencyExchange.Core.Repositories;

namespace CurrencyExchange.Repository.Repositories
{
    public class CryptoCoinPriceRepository : GenericRepository<CryptoCoinPrice>, ICryptoCoinPriceRepository
    {
        public CryptoCoinPriceRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
