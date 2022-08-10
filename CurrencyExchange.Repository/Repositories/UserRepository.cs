using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Repositories;

namespace CurrencyExchange.Repository.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
