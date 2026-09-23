namespace Scheduler.Infrastructure.Entities;

// Status/CompletedAt are updated by the job itself as it runs, not by the enqueuing service.
public class JobRun
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string HangfireJobId { get; set; } = default!;
    public string JobType { get; set; } = default!; // e.g. "GenerateDummyData"
    public string Status { get; set; } = "Processing"; // Processing | Success | Error
    public string? ResultMessage { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
