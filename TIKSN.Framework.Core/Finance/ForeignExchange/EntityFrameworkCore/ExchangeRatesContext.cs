using Microsoft.EntityFrameworkCore;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore
{
    public class ExchangeRatesContext : DbContext
    {
        public ExchangeRatesContext(DbContextOptions<ExchangeRatesContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public virtual DbSet<ExchangeRateEntity> ExchangeRates { get; set; }
        public virtual DbSet<ForeignExchangeEntity> ForeignExchanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRateEntity>(entity =>
            {
                entity.ToTable("ExchangeRates");

                entity.Property(e => e.ID).HasColumnName("ID");

                entity.Property(e => e.AsOn)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.BaseCurrencyCode)
                    .IsRequired()
                    .HasColumnType("STRING (3, 3)");

                entity.Property(e => e.CounterCurrencyCode)
                    .IsRequired()
                    .HasColumnType("STRING (3, 3)");

                entity.Property(e => e.ForeignExchangeID).HasColumnName("ForeignExchangeID");

                entity.Property(e => e.Rate)
                    .IsRequired()
                    .HasColumnType("DECIMAL");

                entity.HasOne(d => d.ForeignExchange)
                    .WithMany(p => p.ExchangeRates)
                    .HasForeignKey(d => d.ForeignExchangeID);
            });

            modelBuilder.Entity<ForeignExchangeEntity>(entity =>
            {
                entity.ToTable("ForeignExchanges");
                entity.Property(e => e.ID).HasColumnName("ID");

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasColumnType("STRING (2, 3)");
            });
        }
    }
}
