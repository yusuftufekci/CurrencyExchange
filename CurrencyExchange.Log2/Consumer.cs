using System.Text;
using CurrencyExchange.Log.Concrete;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CurrencyExchange.Log2
{
    public class Consumer
    {
        private readonly RabbitMqService _rabbitMqService;

        public Consumer(string queueName)
        {
            LoggerManager logger = new LoggerManager();
            _rabbitMqService = new RabbitMqService();

            using (var connection = _rabbitMqService.GetRabbitMqConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channel);
                    // Received event'i sürekli listen modunda olacaktır.
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();

                        var message = Encoding.UTF8.GetString(body);
                        logger.LogInfo(message);

                        Console.WriteLine("{0} isimli queue üzerinden gelen mesaj: \"{1}\"", queueName, message);
                    };

                    channel.BasicConsume(queueName, true, consumer);
                    Console.ReadLine();
                }
            }
        }
    }
}