using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.RabbitMqLogger
{
    public interface ISenderLogger
    {
         void SenderFunction(string queName, string logMessage);

    }
}
