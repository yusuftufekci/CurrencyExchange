using RabbitMQ.Client;

namespace CurrencyExchange.Log2
{
    public class RabbitMqService
    {
        private readonly string _hostName = "host.docker.internal";

        public IConnection GetRabbitMqConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = _hostName
            };

            return connectionFactory.CreateConnection();
        }
    }
}

