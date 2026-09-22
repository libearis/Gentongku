using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduler.Infrastructure.Entities;
using Scheduler.Infrastructure.Persistence;

namespace Scheduler.Infrastructure.Jobs;

/// <summary>
/// Hangfire job body. TODO (next implementation pass, AGENTS.md section 7):
/// generate realistic dummy Catalog/Ordering rows toward the requested row
/// count or estimated-from-storage-size row count. This stub only proves the
/// Hangfire pipeline + job_runs sync round-trip (Processing -> Success/Error).
/// </summary>
public sealed class GenerateDummyDataJob
{
    private readonly SchedulerDbContext _db;
    private readonly ILogger<GenerateDummyDataJob> _logger;

    public GenerateDummyDataJob(SchedulerDbContext db, ILogger<GenerateDummyDataJob> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task RunAsync(Guid jobRunId, long? targetRowCount, long? targetStorageBytes)
    {
        var jobRun = await _db.JobRuns.FirstOrDefaultAsync(j => j.Id == jobRunId);
        if (jobRun is null) return;

        try
        {
            _logger.LogInformation(
                "GenerateDummyDataJob starting (jobRunId={JobRunId}, targetRowCount={TargetRowCount}, targetStorageBytes={TargetStorageBytes}) — TODO: real generation, see AGENTS.md section 7",
                jobRunId, targetRowCount, targetStorageBytes);

            // TODO: actually insert dummy Catalog.Products / Ordering.Orders rows here.
            await Task.Delay(50);

            jobRun.Status = "Success";
            jobRun.ResultMessage = "Stub run completed (no rows generated yet — TODO).";
        }
        catch (Exception ex)
        {
            jobRun.Status = "Error";
            jobRun.ResultMessage = ex.Message;
            _logger.LogError(ex, "GenerateDummyDataJob failed (jobRunId={JobRunId})", jobRunId);
        }
        finally
        {
            jobRun.CompletedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}
