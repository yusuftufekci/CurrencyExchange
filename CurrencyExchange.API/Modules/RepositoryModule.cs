using Autofac;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Repository.Repositories;

namespace CurrencyExchange.API.Modules
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(GenericRepository<>))
                .As(typeof(IGenericRepository<>))
                .InstancePerLifetimeScope();
            builder.RegisterType(typeof(AccountRepository)).As(typeof(IAccountRepository)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(BalanceRepository)).As(typeof(IBalanceRepository)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(LogMessagesRepository))
                .As(typeof(ILogMessagesRepository))
                .InstancePerLifetimeScope();
            builder.RegisterType(typeof(PasswordRepository)).As(typeof(IPasswordRepository)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(ResponseMessageRepository))
                .As(typeof(IResponseMessageRepository))
                .InstancePerLifetimeScope();
            builder.RegisterType(typeof(TokenRepository)).As(typeof(ITokenRepository)).InstancePerLifetimeScope();
            builder.RegisterType(typeof(UserBalanceHistoryRepository))
                .As(typeof(IUserBalanceHistoryRepository))
                .InstancePerLifetimeScope();
            builder.RegisterType(typeof(UserRepository)).As(typeof(IUserRepository)).InstancePerLifetimeScope();
        }
    }
}