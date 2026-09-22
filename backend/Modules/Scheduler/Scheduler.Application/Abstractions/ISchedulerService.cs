using Scheduler.Application.DTOs;

namespace Scheduler.Application.Abstractions;

/// <summary>
/// Public contract for the Scheduler module (AGENTS.md section 7). Enqueues a
/// Hangfire job immediately ("fire and forget" from the caller's perspective)
/// and exposes the job_runs list for the polling Job Monitor UI.
/// The actual dummy-data-generation job BODY is TODO-stubbed in this pass —
/// see Scheduler.Infrastructure/Jobs/GenerateDummyDataJob.cs.
/// </summary>
public interface ISchedulerService
{
    Task<string> EnqueueGenerateDummyDataAsync(GenerateDummyDataRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<JobRunDto>> ListRecentJobsAsync(int take = 20, CancellationToken ct = default);
}
