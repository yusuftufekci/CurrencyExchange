using Autofac;
using CurrencyExchange.Caching.CryptoCoins;

namespace CurrencyExchange.API.Modules
{
    public class CacheModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType(typeof(CryptoCoinPriceServiceWithCaching)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(CryptoCoinServiceWithCaching)).InstancePerLifetimeScope();
        }
    }
}
