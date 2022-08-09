using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Log2
{
    public class RabbitMQService
    {
        private readonly string _hostName = "host.docker.internal";

        public IConnection GetRabbitMQConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = _hostName
            };

            return connectionFactory.CreateConnection();
        }
    }
}

