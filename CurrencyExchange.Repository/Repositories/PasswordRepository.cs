using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Repositories;

namespace CurrencyExchange.Repository.Repositories
{
    public class PasswordRepository : GenericRepository<Password>, IPasswordRepository
    {
        public PasswordRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
