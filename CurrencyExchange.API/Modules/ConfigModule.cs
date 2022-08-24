using Autofac;
using CurrencyExchange.Core.ConfigModels;

namespace CurrencyExchange.API.Modules
{
    public class ConfigModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(AppSettings)).InstancePerMatchingLifetimeScope();
            builder.RegisterType(typeof(RabbitMqSettings)).InstancePerMatchingLifetimeScope();
            builder.RegisterType(typeof(ControlCryptoCoinAmountSettings)).InstancePerMatchingLifetimeScope();

        }

    }
}
