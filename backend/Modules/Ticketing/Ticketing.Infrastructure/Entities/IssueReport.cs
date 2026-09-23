namespace Ticketing.Infrastructure.Entities;

// Local log of tickets filed against the external TaskFlow app; this module has no other tables.
public class IssueReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string IssueId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Severity { get; set; } = default!;
    public string ReportedBy { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
