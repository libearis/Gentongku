namespace Scheduler.Application.DTOs;

public enum DummyDataTable
{
    Category,
    Product,
    Order,
}

public sealed record GenerateDummyDataRequest(DummyDataTable Table, long? TargetRowCount, long? TargetStorageBytes);

public sealed record JobRunDto(Guid Id, string HangfireJobId, string JobType, string Status, string? ResultMessage, DateTimeOffset CreatedAt, DateTimeOffset? CompletedAt);
