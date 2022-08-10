using CurrencyExchange.Core.Entities.CryptoCoins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CurrencyExchange.Repository.Configurations
{
    internal class CryptoCoinConfiguration : IEntityTypeConfiguration<CryptoCoin>
    {
        public void Configure(EntityTypeBuilder<CryptoCoin> builder)
        {
            builder.Property(p => p.CoinName).IsRequired().HasMaxLength(10);
            builder.Property(p => p.ModifiedDate).IsRequired();
            builder.Property(p => p.CreatedDate).IsRequired();
        }
    }
}
