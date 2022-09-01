using Autofac;
using CurrencyExchange.API.Filters;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using CurrencyExchange.Repository.UnitOfWorks;
using CurrencyExchange.Service.CommonFunction;
using CurrencyExchange.Service.LogFacade;
using CurrencyExchange.Service.Services;
using Module = Autofac.Module;

namespace CurrencyExchange.API.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();


           // builder.RegisterGeneric(typeof(Service<>)).As(typeof(IService<>)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(AccountService)).As(typeof(IAccountService)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(AuthenticationService)).As(typeof(IAuthenticationService)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(BuyCryptoCoinService)).As(typeof(IBuyCryptoCoinService)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(SellCryptoCoinService)).As(typeof(ISellCryptoCoinService)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(UserInformationService)).As(typeof(IUserInformationService)).InstancePerLifetimeScope();


            builder.RegisterType(typeof(CommonFunctions)).As(typeof(ICommonFunctions)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(TokenControlFilter<>)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(LogResponseFacade)).InstancePerLifetimeScope();



        }
    }
}