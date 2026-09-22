namespace Ticketing.Infrastructure.Entities;

/// <summary>
/// Minimal local log of tickets filed against the external TaskFlow app
/// (AGENTS.md section 8: "store the returned issue_id locally and surface it
/// in the ticket history list"). Ticketing has no other tables per AGENTS.md
/// section 3's folder tree — this is the one reasonable addition called out
/// in the task brief.
/// </summary>
public class IssueReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string IssueId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Severity { get; set; } = default!;
    public string ReportedBy { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
