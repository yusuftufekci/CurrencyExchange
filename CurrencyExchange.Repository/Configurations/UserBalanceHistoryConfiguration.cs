﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            builder.Property(x => x.ExchangedCoinName).IsRequired().HasMaxLength(10);
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.ModifiedDate).IsRequired();
        }
    }
}
