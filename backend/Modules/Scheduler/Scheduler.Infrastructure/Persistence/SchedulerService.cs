using Hangfire;
using Microsoft.EntityFrameworkCore;
using Scheduler.Application.Abstractions;
using Scheduler.Application.DTOs;
using Scheduler.Infrastructure.Entities;
using Scheduler.Infrastructure.Jobs;

namespace Scheduler.Infrastructure.Persistence;

public sealed class SchedulerService : ISchedulerService
{
    private readonly SchedulerDbContext _db;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public SchedulerService(SchedulerDbContext db, IBackgroundJobClient backgroundJobClient)
    {
        _db = db;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<string> EnqueueGenerateDummyDataAsync(GenerateDummyDataRequest request, CancellationToken ct = default)
    {
        var jobRun = new JobRun
        {
            HangfireJobId = string.Empty,
            JobType = $"GenerateDummyData:{request.Table}",
            Status = "Processing"
        };
        _db.JobRuns.Add(jobRun);
        await _db.SaveChangesAsync(ct);

        var hangfireId = _backgroundJobClient.Enqueue<GenerateDummyDataJob>(
            job => job.RunAsync(jobRun.Id, request.Table, request.TargetRowCount, request.TargetStorageBytes));

        jobRun.HangfireJobId = hangfireId;
        await _db.SaveChangesAsync(ct);

        return hangfireId;
    }

    public async Task<IReadOnlyList<JobRunDto>> ListRecentJobsAsync(int take = 20, CancellationToken ct = default) =>
        await _db.JobRuns.AsNoTracking()
            .OrderByDescending(j => j.CreatedAt)
            .Take(take)
            .Select(j => new JobRunDto(j.Id, j.HangfireJobId, j.JobType, j.Status, j.ResultMessage, j.CreatedAt, j.CompletedAt))
            .ToListAsync(ct);
}
