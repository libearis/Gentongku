namespace Scheduler.Application.DTOs;

public sealed record GenerateDummyDataRequest(long? TargetRowCount, long? TargetStorageBytes);

public sealed record JobRunDto(Guid Id, string HangfireJobId, string JobType, string Status, string? ResultMessage, DateTimeOffset CreatedAt, DateTimeOffset? CompletedAt);
