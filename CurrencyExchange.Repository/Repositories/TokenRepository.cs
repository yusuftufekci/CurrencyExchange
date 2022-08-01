using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Repository.Repositories
{
    public class TokenRepository : GenericRepository<UserToken>, ITokenRepository
    {
        public TokenRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
