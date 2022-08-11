using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.Core.Entities.LogMessages;
using CurrencyExchange.Core.Repositories;

namespace CurrencyExchange.Repository.Repositories
{
    public class LogMessagesRepository : GenericRepository<LogMessages>, ILogMessagesRepository
    {
        public LogMessagesRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
