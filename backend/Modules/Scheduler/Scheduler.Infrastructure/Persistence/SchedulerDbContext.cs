using Microsoft.EntityFrameworkCore;
using Scheduler.Infrastructure.Entities;

namespace Scheduler.Infrastructure.Persistence;

/// <summary>DbContext scoped to the `scheduler` Postgres schema.</summary>
public class SchedulerDbContext : DbContext
{
    public const string Schema = "scheduler";

    public SchedulerDbContext(DbContextOptions<SchedulerDbContext> options) : base(options) { }

    public DbSet<JobRun> JobRuns => Set<JobRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<JobRun>(b =>
        {
            b.ToTable("job_runs");
            b.HasKey(j => j.Id);
            b.Property(j => j.HangfireJobId).HasMaxLength(100).IsRequired();
            b.Property(j => j.JobType).HasMaxLength(100).IsRequired();
            b.Property(j => j.Status).HasMaxLength(20).IsRequired();
            b.HasIndex(j => j.CreatedAt);

            b.Property(j => j.Id).HasColumnOrder(0);
            b.Property(j => j.CreatedAt).HasColumnOrder(1);
            b.Property(j => j.CreatedBy).HasColumnOrder(2);
            b.Property(j => j.UpdatedAt).HasColumnOrder(3);
            b.Property(j => j.UpdatedBy).HasColumnOrder(4);
            b.Property(j => j.DeletedAt).HasColumnOrder(5);
            b.Property(j => j.DeletedBy).HasColumnOrder(6);
        });
    }
}
