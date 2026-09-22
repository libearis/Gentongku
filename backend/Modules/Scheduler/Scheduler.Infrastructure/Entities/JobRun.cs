namespace Scheduler.Infrastructure.Entities;

/// <summary>
/// Thin sync record keyed by the Hangfire job id, kept in sync from within the
/// job itself (AGENTS.md section 7: "persist a thin job_runs row ... keep it in
/// sync from within the job itself"). This does NOT replace Hangfire's own
/// storage/dashboard — it only powers the frontend's polling Job Monitor list.
/// </summary>
public class JobRun
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string HangfireJobId { get; set; } = default!;
    public string JobType { get; set; } = default!; // e.g. "GenerateDummyData"
    public string Status { get; set; } = "Processing"; // Processing | Success | Error
    public string? ResultMessage { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
}
