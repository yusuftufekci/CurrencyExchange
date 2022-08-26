using Autofac;
using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using CurrencyExchange.Repository.UnitOfWorks;
using CurrencyExchange.Service.CommonFunction;
using CurrencyExchange.Service.Services;
using Module = Autofac.Module;

namespace CurrencyExchange.API.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();


            builder.RegisterGeneric(typeof(Service<>)).As(typeof(IService<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(AccountService<>)).As(typeof(IAccountService<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(AuthenticationService<>)).As(typeof(IAuthenticationService<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(BuyCryptoCoinService<>)).As(typeof(IBuyCryptoCoinService<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(SellCryptoCoinService<>)).As(typeof(ISellCryptoCoinService<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(UserInformationService<>)).As(typeof(IUserInformationService<>)).InstancePerLifetimeScope();


            builder.RegisterType(typeof(CommonFunctions)).As(typeof(ICommonFunctions)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(TokenControlFilter<>)).InstancePerLifetimeScope();
            

        }
    }
}