using System.Text;
using CurrencyExchange.Core.ConfigModels;
using CurrencyExchange.Core.RabbitMqLogger;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CurrencyExchange.Service.RabbitMqLogger
{
    public class SenderLogger : ISenderLogger
    {
        private readonly RabbitMqSettings _rabbitMqSettings;

        public SenderLogger(IOptions<RabbitMqSettings> rabbitMqSettings)
        {
            _rabbitMqSettings = rabbitMqSettings.Value;
        }

        public void SenderFunction(string queName, string logMessage)
        {
            var factory = new ConnectionFactory() { HostName = _rabbitMqSettings.Host };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queName, durable: false, exclusive: false, autoDelete: false,
                    arguments: null);

                var message = logMessage;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: queName, basicProperties: null, body: body);
            }
        }
    }
}