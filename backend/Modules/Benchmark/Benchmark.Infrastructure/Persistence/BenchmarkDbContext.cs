using Benchmark.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Benchmark.Infrastructure.Persistence;

/// <summary>
/// DbContext scoped to the `benchmark` Postgres schema. Only owns the two
/// write-benchmark tables (AGENTS.md section 6.2) — the read benchmark queries
/// Catalog/Ordering tables directly through their own public query contracts,
/// it does not duplicate their schemas.
/// </summary>
public class BenchmarkDbContext : DbContext
{
    public const string Schema = "benchmark";

    public BenchmarkDbContext(DbContextOptions<BenchmarkDbContext> options) : base(options) { }

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
            // Deliberately zero secondary indexes (AGENTS.md section 6.2).
            b.Property(o => o.Status).HasMaxLength(20);
            b.Property(o => o.TotalAmount).HasColumnType("numeric(18,2)");
        });
    }
}
