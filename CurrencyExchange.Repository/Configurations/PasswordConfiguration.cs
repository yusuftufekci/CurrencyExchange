using CurrencyExchange.Core.Entities.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CurrencyExchange.Repository.Configurations
{
    internal class PasswordConfiguration : IEntityTypeConfiguration<Password>
    {
       
        public void Configure(EntityTypeBuilder<Password> builder)
        {
            builder.Property(p => p.PasswordHash).IsRequired();
            builder.Property(p => p.PasswordSalt).IsRequired();
            builder.Property(p => p.ModifiedDate).IsRequired();
            builder.Property(p => p.CreatedDate).IsRequired();

        }
    }
}
