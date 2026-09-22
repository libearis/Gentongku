using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

/// <summary>
/// DbContext scoped to the `catalog` Postgres schema. Also defines the indexes
/// referenced by the Benchmark module's read benchmark (AGENTS.md section 6.1):
/// Produk (Product) indexes on the columns actually queryable from the column
/// picker; deliberately no index on a "notes"-equivalent free-text column.
/// </summary>
public class CatalogDbContext : DbContext
{
    public const string Schema = "catalog";

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SellerProfile> SellerProfiles => Set<SellerProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Category>(b =>
        {
            b.ToTable("categories");
            b.HasKey(c => c.Id);
            b.Property(c => c.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.ToTable("products");
            b.HasKey(p => p.Id);
            b.Property(p => p.Name).HasMaxLength(300).IsRequired();
            b.Property(p => p.Description).HasColumnType("text"); // intentionally unindexed
            b.Property(p => p.Price).HasColumnType("numeric(18,2)");
            b.HasIndex(p => p.CategoryId);
            b.HasIndex(p => p.SellerId);
            b.HasIndex(p => p.CreatedAt);
        });

        modelBuilder.Entity<SellerProfile>(b =>
        {
            b.ToTable("seller_profiles");
            b.HasKey(s => s.Id);
            b.HasIndex(s => s.UserId).IsUnique();
            b.Property(s => s.StoreName).HasMaxLength(200).IsRequired();
        });
    }
}
