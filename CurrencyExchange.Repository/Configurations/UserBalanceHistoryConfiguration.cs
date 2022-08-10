using CurrencyExchange.Core.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CurrencyExchange.Repository.Configurations
{
    internal class UserBalanceHistoryConfiguration : IEntityTypeConfiguration<UserBalanceHistory>
    {
        public void Configure(EntityTypeBuilder<UserBalanceHistory> builder)
        {
            builder.Property(x => x.MessageForChanging).IsRequired().HasMaxLength(500);
            builder.Property(x => x.ChangedAmount).IsRequired().HasColumnType("decimal(15,4)"); ;
            builder.Property(x => x.ChangedAmountSoldCryptoCoin).IsRequired().HasColumnType("decimal(15,4)"); ;
            builder.Property(x => x.BoughtCryptoCoin).IsRequired().HasMaxLength(10);
            builder.Property(x => x.SoldCryptoCoin).IsRequired().HasMaxLength(10);
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.ModifiedDate).IsRequired();
        }
    }
}
