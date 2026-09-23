using Microsoft.EntityFrameworkCore;
using Ticketing.Infrastructure.Entities;

namespace Ticketing.Infrastructure.Persistence;

public class TicketingDbContext : DbContext
{
    public const string Schema = "ticketing";

    public TicketingDbContext(DbContextOptions<TicketingDbContext> options) : base(options) { }

    public DbSet<IssueReport> IssueReports => Set<IssueReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<IssueReport>(b =>
        {
            b.ToTable("issue_reports");
            b.HasKey(i => i.Id);
            b.Property(i => i.IssueId).HasMaxLength(100).IsRequired();
            b.Property(i => i.Title).HasMaxLength(300).IsRequired();
            b.Property(i => i.Severity).HasMaxLength(20).IsRequired();
            b.Property(i => i.ReportedBy).HasMaxLength(200).IsRequired();
            b.HasIndex(i => i.CreatedAt);

            b.Property(i => i.Id).HasColumnOrder(0);
            b.Property(i => i.CreatedAt).HasColumnOrder(1);
            b.Property(i => i.CreatedBy).HasColumnOrder(2);
            b.Property(i => i.UpdatedAt).HasColumnOrder(3);
            b.Property(i => i.UpdatedBy).HasColumnOrder(4);
            b.Property(i => i.DeletedAt).HasColumnOrder(5);
            b.Property(i => i.DeletedBy).HasColumnOrder(6);
        });
    }
}
