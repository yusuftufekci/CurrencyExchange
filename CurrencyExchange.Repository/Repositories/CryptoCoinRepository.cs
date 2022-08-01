using CurrencyExchange.Core.Entities.CryptoCoins;
using CurrencyExchange.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Repository.Repositories
{
    public class CryptoCoinRepository : GenericRepository<CryptoCoin>, ICryptoCoinRepository
    {
        public CryptoCoinRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
