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
            _ = modelBuilder.Entity<ExchangeRateEntity>(entity =>
              {
                  _ = entity.ToTable("ExchangeRates");

                  _ = entity.Property(e => e.ID).HasColumnName("ID");

                  _ = entity.Property(e => e.AsOn)
                      .IsRequired()
                      .HasColumnType("DATETIME");

                  _ = entity.Property(e => e.BaseCurrencyCode)
                      .IsRequired()
                      .HasColumnType("STRING (3, 3)");

                  _ = entity.Property(e => e.CounterCurrencyCode)
                      .IsRequired()
                      .HasColumnType("STRING (3, 3)");

                  _ = entity.Property(e => e.ForeignExchangeID).HasColumnName("ForeignExchangeID");

                  _ = entity.Property(e => e.Rate)
                      .IsRequired()
                      .HasColumnType("DECIMAL");

                  _ = entity.HasOne(d => d.ForeignExchange)
                      .WithMany(p => p.ExchangeRates)
                      .HasForeignKey(d => d.ForeignExchangeID);
              });

            _ = modelBuilder.Entity<ForeignExchangeEntity>(entity =>
              {
                  _ = entity.ToTable("ForeignExchanges");
                  _ = entity.Property(e => e.ID).HasColumnName("ID");

                  _ = entity.Property(e => e.CountryCode)
                      .IsRequired()
                      .HasColumnType("STRING (2, 3)");
              });
        }
    }
}
