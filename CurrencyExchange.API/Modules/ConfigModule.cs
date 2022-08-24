using Autofac;
using CurrencyExchange.Core.ConfigModels;

namespace CurrencyExchange.API.Modules
{
    public class ConfigModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(AppSettings)).SingleInstance();
            builder.RegisterType(typeof(RabbitMqSettings)).SingleInstance();
            builder.RegisterType(typeof(ControlCryptoCoinAmountSettings)).SingleInstance();

        }

    }
}
