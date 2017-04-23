using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Web_Service.Data.Entities
{
	public partial class InternationalizationContext : DbContext
	{
		public InternationalizationContext(DbContextOptions<InternationalizationContext> options) : base(options)
		{

		}
		public virtual DbSet<CultureEntity> Cultures { get; set; }
		public virtual DbSet<CurrencyEntity> Currencies { get; set; }
		public virtual DbSet<RegionEntity> Regions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CultureEntity>(entity =>
			{
				entity.ForSqlServerToTable("Cultures");

				entity.HasIndex(e => e.Code)
					.HasName("IX_Cultures_Code")
					.IsUnique();

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Code)
					.IsRequired()
					.HasColumnType("nchar(10)");

				entity.Property(e => e.EnglishName)
					.IsRequired()
					.HasColumnType("nchar(255)");

				entity.Property(e => e.Lcid).HasColumnName("LCID");

				entity.Property(e => e.NativeName)
					.IsRequired()
					.HasColumnType("nchar(255)");

				entity.Property(e => e.ParentId).HasColumnName("ParentID");

				entity.Property(e => e.RegionId).HasColumnName("RegionID");

				entity.HasOne(d => d.Parent)
					.WithMany(p => p.Children)
					.HasForeignKey(d => d.ParentId)
					.HasConstraintName("FK_Cultures_Cultures");

				entity.HasOne(d => d.Region)
					.WithMany(p => p.Cultures)
					.HasForeignKey(d => d.RegionId)
					.HasConstraintName("FK_Cultures_Regions");
			});

			modelBuilder.Entity<CurrencyEntity>(entity =>
			{
				entity.ForSqlServerToTable("Currencies");

				entity.HasIndex(e => e.Code)
					.HasName("IX_Currencies_Code")
					.IsUnique();

				entity.HasIndex(e => e.Number)
					.HasName("IX_Currencies_Number")
					.IsUnique();

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Code)
					.IsRequired()
					.HasColumnType("nchar(10)");

				entity.Property(e => e.CurrencySymbol)
					.IsRequired()
					.HasColumnType("nchar(10)");

				entity.Property(e => e.IsCurrent)
					.IsRequired()
					.HasColumnType("bit");

				entity.Property(e => e.IsFund)
					.IsRequired()
					.HasColumnType("bit");
			});

			modelBuilder.Entity<RegionEntity>(entity =>
			{
				entity.ForSqlServerToTable("Regions");

				entity.HasIndex(e => e.Code)
					.HasName("IX_Regions_Code")
					.IsUnique();

				entity.HasIndex(e => e.GeoId)
					.HasName("IX_Regions_GeoId")
					.IsUnique();

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Code)
					.IsRequired()
					.HasColumnType("nchar(10)");

				entity.Property(e => e.CurrencyId).HasColumnName("CurrencyID");

				entity.Property(e => e.EnglishName)
					.IsRequired()
					.HasColumnType("nchar(255)");

				entity.Property(e => e.NativeName)
					.IsRequired()
					.HasColumnType("nchar(255)");

				entity.HasOne(d => d.Currency)
					.WithMany(p => p.Regions)
					.HasForeignKey(d => d.CurrencyId)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_Regions_Currencies");
			});
		}
	}
}