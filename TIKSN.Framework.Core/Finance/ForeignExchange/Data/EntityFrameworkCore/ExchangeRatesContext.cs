using Microsoft.EntityFrameworkCore;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

public class ExchangeRatesContext : DbContext
{
    public ExchangeRatesContext(DbContextOptions<ExchangeRatesContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public virtual DbSet<ExchangeRateDataEntity> ExchangeRates { get; set; }
    public virtual DbSet<ForeignExchangeDataEntity> ForeignExchanges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        _ = modelBuilder.Entity<ExchangeRateDataEntity>(entity =>
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

        _ = modelBuilder.Entity<ForeignExchangeDataEntity>(entity =>
          {
              _ = entity.ToTable("ForeignExchanges");
              _ = entity.Property(e => e.ID).HasColumnName("ID");

              _ = entity.Property(e => e.CountryCode)
                  .IsRequired()
                  .HasColumnType("STRING (2, 3)");
          });
    }
}
