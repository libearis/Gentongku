using Microsoft.EntityFrameworkCore;
using Ticketing.Infrastructure.Entities;

namespace Ticketing.Infrastructure.Persistence;

/// <summary>DbContext scoped to the `ticketing` Postgres schema.</summary>
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
        });
    }
}
