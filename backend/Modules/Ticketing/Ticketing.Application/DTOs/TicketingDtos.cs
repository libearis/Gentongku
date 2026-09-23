namespace Ticketing.Application.DTOs;

// Mirrors CreateIssueRequest from issue_intake.proto, kept as a plain DTO so callers never touch the generated gRPC message types.
public sealed record CreateTicketRequest(string Title, string? Description, string? Severity, string? TriggerType, string ReportedBy);

public sealed record CreateTicketResult(string IssueId, string Status);

public sealed record TicketHistoryItemDto(Guid Id, string IssueId, string Title, string Severity, string ReportedBy, DateTimeOffset CreatedAt);
