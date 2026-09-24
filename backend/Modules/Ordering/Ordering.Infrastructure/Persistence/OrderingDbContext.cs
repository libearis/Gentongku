using BuildingBlocks.Entities;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence;

public class OrderingDbContext(DbContextOptions<OrderingDbContext> options) : DbContext(options)
{
    public const string Schema = "ordering";

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Order>(b =>
        {
            b.ToTable("orders");
            b.HasKey(o => o.Id);
            b.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            b.Property(o => o.TotalAmount).HasColumnType("numeric(18,2)");
            b.Property(o => o.Notes).HasColumnType("text"); // intentionally unindexed
            b.HasIndex(o => o.Status);
            b.HasIndex(o => o.CreatedAt);
            b.HasIndex(o => o.BuyerId);
        });

        AuditColumns.Configure(modelBuilder);
    }
}

// Duplicated per-module instead of shared via BuildingBlocks so BuildingBlocks never takes an EF Core dependency.
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
