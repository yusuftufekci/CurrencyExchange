using RabbitMQ.Client;

namespace CurrencyExchange.Log2
{
    public class RabbitMqService
    {
        private readonly string _hostName = "localhost";

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

