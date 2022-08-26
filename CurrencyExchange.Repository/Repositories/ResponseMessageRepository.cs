using CurrencyExchange.Core.Entities.Log;
using CurrencyExchange.Core.Repositories;

namespace CurrencyExchange.Repository.Repositories
{

    public class ResponseMessageRepository : GenericRepository<ResponseMessages>, IResponseMessageRepository
    {
        public ResponseMessageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
