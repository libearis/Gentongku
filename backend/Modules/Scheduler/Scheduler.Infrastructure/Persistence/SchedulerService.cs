using Hangfire;
using Microsoft.EntityFrameworkCore;
using Scheduler.Application.Abstractions;
using Scheduler.Application.DTOs;
using Scheduler.Infrastructure.Entities;
using Scheduler.Infrastructure.Jobs;

namespace Scheduler.Infrastructure.Persistence;

public sealed class SchedulerService(SchedulerDbContext db, IBackgroundJobClient backgroundJobClient) : ISchedulerService
{
    public async Task<string> EnqueueGenerateDummyDataAsync(GenerateDummyDataRequest request, CancellationToken ct = default)
    {
        var jobRun = new JobRun
        {
            HangfireJobId = string.Empty,
            JobType = $"GenerateDummyData:{request.Table}",
            Status = "Processing"
        };
        db.JobRuns.Add(jobRun);
        await db.SaveChangesAsync(ct);

        var hangfireId = backgroundJobClient.Enqueue<GenerateDummyDataJob>(
            job => job.RunAsync(jobRun.Id, request.Table, request.TargetRowCount, request.TargetStorageBytes));

        jobRun.HangfireJobId = hangfireId;
        await db.SaveChangesAsync(ct);

        return hangfireId;
    }

    public async Task<IReadOnlyList<JobRunDto>> ListRecentJobsAsync(int take = 20, CancellationToken ct = default) =>
        await db.JobRuns.AsNoTracking()
            .OrderByDescending(j => j.CreatedAt)
            .Take(take)
            .Select(j => new JobRunDto(j.Id, j.HangfireJobId, j.JobType, j.Status, j.ResultMessage, j.CreatedAt, j.CompletedAt))
            .ToListAsync(ct);
}
