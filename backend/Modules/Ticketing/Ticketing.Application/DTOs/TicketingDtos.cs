namespace Ticketing.Application.DTOs;

/// <summary>
/// Mirrors CreateIssueRequest from issue_intake.proto (docs/external-issue-intake.md
/// section 2/3). Kept as a plain Application-layer DTO so callers never touch the
/// generated gRPC message types directly.
/// </summary>
public sealed record CreateTicketRequest(string Title, string? Description, string? Severity, string? TriggerType, string ReportedBy);

public sealed record CreateTicketResult(string IssueId, string Status);

public sealed record TicketHistoryItemDto(Guid Id, string IssueId, string Title, string Severity, string ReportedBy, DateTimeOffset CreatedAt);
