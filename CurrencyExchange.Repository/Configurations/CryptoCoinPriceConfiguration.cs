using CurrencyExchange.Core.Entities.CryptoCoins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CurrencyExchange.Repository.Configurations
{
    internal class CryptoCoinPriceConfiguration : IEntityTypeConfiguration<CryptoCoinPrice>
    {
 

        public void Configure(EntityTypeBuilder<CryptoCoinPrice> builder)
        {
            builder.Property(p => p.Symbol).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Price).IsRequired();
            builder.Property(p => p.ModifiedDate).IsRequired();
            builder.Property(p => p.CreatedDate).IsRequired();
        }
    }
}
