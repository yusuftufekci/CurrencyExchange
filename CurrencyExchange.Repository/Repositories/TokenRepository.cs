using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Repositories;

namespace CurrencyExchange.Repository.Repositories
{
    public class TokenRepository : GenericRepository<UserToken>, ITokenRepository
    {
        public TokenRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
