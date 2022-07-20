using CurrencyExchange2.Model.Authentication;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options ) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }

        public DbSet<UserToken> UserTokens { get; set; }

}
}
