using System.Text;
using CurrencyExchange.Core.RabbitMqLogger;
using RabbitMQ.Client;

namespace CurrencyExchange.Service.RabbitMqLogger
{
    public class SenderLogger : ISenderLogger
    {
        public  void SenderFunction(string queName, string logMessage)
        {
            var factory = new ConnectionFactory() { HostName = "host.docker.internal" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var message = logMessage;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queName,
                                     basicProperties: null,
                                     body: body);
            }

        }

    }
}
