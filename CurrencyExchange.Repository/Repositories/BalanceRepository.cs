using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Repository.Repositories
{
    public class BalanceRepository : GenericRepository<Balance>, IBalanceRepository
    {
        public BalanceRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
