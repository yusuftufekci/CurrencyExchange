namespace CurrencyExchange.Core.RabbitMqLogger
{
    public interface ISenderLogger
    {
         void SenderFunction(string queName, string logMessage);

    }
}
