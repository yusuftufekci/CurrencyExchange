using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using CurrencyExchange.Core.Entities.Log;
using CurrencyExchange.Core.Entities.LogMessages;

namespace CurrencyExchange.Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }

        public DbSet<UserToken> UserTokens { get; set; }

        public DbSet<Password> PasswordInfos { get; set; }


        public DbSet<Account> Accounts { get; set; }
        public DbSet<ResponseMessages> ResponseMessages { get; set; }
        public DbSet<LogMessages> LogMessages { get; set; }

        public DbSet<Balance> Balances { get; set; }
        public DbSet<UserBalanceHistory> UserBalanceHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CryptoCurrency;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}