using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore
{
    public class ExchangeRatesContext : DbContext
    {
        public virtual DbSet<ExchangeRate> ExchangeRates { get; set; }
        public virtual DbSet<ForeignExchange> ForeignExchanges { get; set; }

        public ExchangeRatesContext(DbContextOptions<ExchangeRatesContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.ToTable("ExchangeRates");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AsOn)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.BaseCurrencyCode)
                    .IsRequired()
                    .HasColumnType("STRING (3, 3)");

                entity.Property(e => e.CounterCurrencyCode)
                    .IsRequired()
                    .HasColumnType("STRING (3, 3)");

                entity.Property(e => e.ForeignExchangeId).HasColumnName("ForeignExchangeID");

                entity.Property(e => e.Rate)
                    .IsRequired()
                    .HasColumnType("DECIMAL");

                entity.HasOne(d => d.ForeignExchange)
                    .WithMany(p => p.ExchangeRates)
                    .HasForeignKey(d => d.ForeignExchangeId);
            });

            modelBuilder.Entity<ForeignExchange>(entity =>
            {
                entity.ToTable("ForeignExchanges");
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasColumnType("STRING (2, 3)");
            });
        }
    }
}