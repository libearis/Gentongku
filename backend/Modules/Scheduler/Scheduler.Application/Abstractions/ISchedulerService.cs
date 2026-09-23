using Scheduler.Application.DTOs;

namespace Scheduler.Application.Abstractions;

public interface ISchedulerService
{
    Task<string> EnqueueGenerateDummyDataAsync(GenerateDummyDataRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<JobRunDto>> ListRecentJobsAsync(int take = 20, CancellationToken ct = default);
}
