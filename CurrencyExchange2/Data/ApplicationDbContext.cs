using CurrencyExchange2.Model.Authentication;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange2.Model.Crypto;
using CurrencyExchange2.Model.Account;

namespace CurrencyExchange2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options ) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }

        public DbSet<UserToken> UserTokens { get; set; }

        public DbSet<PasswordInfo> PasswordInfos { get; set; }

        public DbSet<CryptoCoin> CryptoCoins { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Balance> Balances { get; set; }


    }
}
