using Benchmark.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Benchmark.Infrastructure.Persistence;

// Only owns the two write-benchmark tables; read benchmarks query Catalog/Ordering directly instead of duplicating their schemas.
public class BenchmarkDbContext(DbContextOptions<BenchmarkDbContext> options) : DbContext(options)
{
    public const string Schema = "benchmark";

    public DbSet<OrderIndexedRow> OrdersIndexed => Set<OrderIndexedRow>();
    public DbSet<OrderPlainRow> OrdersPlain => Set<OrderPlainRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<OrderIndexedRow>(b =>
        {
            b.ToTable("orders_indexed");
            b.HasKey(o => o.Id);
            b.Property(o => o.Status).HasMaxLength(20);
            b.Property(o => o.TotalAmount).HasColumnType("numeric(18,2)");
            b.HasIndex(o => o.Status);
            b.HasIndex(o => o.CreatedAt);
            b.HasIndex(o => o.BuyerId);
        });

        modelBuilder.Entity<OrderPlainRow>(b =>
        {
            b.ToTable("orders_plain");
            b.HasKey(o => o.Id);
            // Deliberately zero secondary indexes — this is the "no index" comparison arm.
            b.Property(o => o.Status).HasMaxLength(20);
            b.Property(o => o.TotalAmount).HasColumnType("numeric(18,2)");
        });

        // orders_indexed/orders_plain deliberately skip the standard audit columns (created_by/updated_at/updated_by/deleted_at/deleted_by) — they're timing-sensitive write-benchmark rows, not domain entities.
    }
}
