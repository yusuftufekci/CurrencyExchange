﻿using CurrencyExchange.Core.Entities.Account;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Entities.CryptoCoins;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public DbSet<CryptoCoinPrice> CryptoCoinPrices { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Balance> Balances { get; set; }
        public DbSet<CryptoCoin> CryptoCoins { get; set; }
        public DbSet<UserBalanceHistory> UserBalanceHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server =172.17.0.3, 1433; Database=CryptoCurrency; Trusted_Connection=False; User Id=sa; Password=Karadeniz61; MultipleActiveResultSets=True");
        }
    }
}

