using BuildingBlocks.Entities;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

// One schema per module, no cross-schema FKs.
public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public const string Schema = "identity";

    public DbSet<User> Users => Set<User>();
    public DbSet<Wallet> Wallets => Set<Wallet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(u => u.Id);
            b.Property(u => u.Username).HasMaxLength(64).IsRequired();
            b.HasIndex(u => u.Username).IsUnique();
            b.Property(u => u.Email).HasMaxLength(256).IsRequired();
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.PasswordHash).IsRequired();
            b.Property(u => u.DisplayName).HasMaxLength(200).IsRequired();
            b.Property(u => u.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
            b.Property(u => u.IsActive).IsRequired();
            b.Property(u => u.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Wallet>(b =>
        {
            b.ToTable("wallets");
            b.HasKey(w => w.Id);
            b.HasIndex(w => w.UserId).IsUnique();
            b.Property(w => w.Balance).HasColumnType("numeric(18,2)");
        });

        AuditColumns.Configure(modelBuilder);
    }
}

// Duplicated per module rather than shared via BuildingBlocks, so BuildingBlocks (referenced by Domain projects too) never takes an EF Core dependency.
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
