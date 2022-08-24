using Autofac;
using CurrencyExchange.Core.RabbitMqLogger;
using CurrencyExchange.Service.RabbitMqLogger;

namespace CurrencyExchange.API.Modules
{
    public class LogModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(SenderLogger)).As(typeof(ISenderLogger)).SingleInstance();

        }
    }
}
