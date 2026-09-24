using BuildingBlocks.Entities;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

// Product indexes match the columns queryable from the Benchmark module's column picker (AGENTS.md section 6.1).
public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public const string Schema = "catalog";

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

        AuditColumns.Configure(modelBuilder);
    }
}

// Duplicated per-module rather than shared via BuildingBlocks, so BuildingBlocks (referenced by Domain projects too) never takes an EF Core dependency.
internal static class AuditColumns
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType)) continue;

            entityType.FindProperty(nameof(BaseEntity.Id))!.SetColumnOrder(0);
            entityType.FindProperty(nameof(BaseEntity.CreatedAt))!.SetColumnOrder(1);
            entityType.FindProperty(nameof(BaseEntity.CreatedBy))!.SetColumnOrder(2);
            entityType.FindProperty(nameof(BaseEntity.UpdatedAt))!.SetColumnOrder(3);
            entityType.FindProperty(nameof(BaseEntity.UpdatedBy))!.SetColumnOrder(4);
            entityType.FindProperty(nameof(BaseEntity.DeletedAt))!.SetColumnOrder(5);
            entityType.FindProperty(nameof(BaseEntity.DeletedBy))!.SetColumnOrder(6);
        }
    }
}
