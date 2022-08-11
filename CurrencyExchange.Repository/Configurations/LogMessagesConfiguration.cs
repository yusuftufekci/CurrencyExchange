using CurrencyExchange.Core.Entities.LogMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CurrencyExchange.Repository.Configurations
{
    public class LogMessagesConfiguration : IEntityTypeConfiguration<LogMessages>
    {
        public void Configure(EntityTypeBuilder<LogMessages> builder)
        {
            builder.Property(x => x.Key).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Value).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Language).IsRequired().HasMaxLength(20);
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.ModifiedDate).IsRequired();

        }
    }
}
