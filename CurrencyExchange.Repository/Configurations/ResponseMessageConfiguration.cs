using CurrencyExchange.Core.Entities.Log;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CurrencyExchange.Repository.Configurations
{

    internal class ResponseMessageConfiguration : IEntityTypeConfiguration<ResponseMessages>
    {
        public void Configure(EntityTypeBuilder<ResponseMessages> builder)
        {
            builder.Property(x => x.Key).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Value).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Language).IsRequired().HasMaxLength(20);
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.ModifiedDate).IsRequired();

        }
    }
}
