using Catalog.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Application.Abstractions;
using Scheduler.Application.DTOs;
using Scheduler.Infrastructure.Persistence;

namespace Scheduler.Infrastructure.Jobs;

public sealed class GenerateDummyDataJob
{
    // Average per-row size estimate, not byte-precise.
    private const int AvgRowBytes = 420;

    // Dev-safety cap to avoid accidentally inserting millions of rows into a local Postgres instance.
    private const int MaxRowsPerRun = 50_000;

    private readonly SchedulerDbContext _db;
    private readonly ICatalogDummyDataGenerator _catalogGenerator;
    private readonly IOrderingDummyDataGenerator _orderingGenerator;
    private readonly ILogger<GenerateDummyDataJob> _logger;

    public GenerateDummyDataJob(
        SchedulerDbContext db,
        ICatalogDummyDataGenerator catalogGenerator,
        IOrderingDummyDataGenerator orderingGenerator,
        ILogger<GenerateDummyDataJob> logger)
    {
        _db = db;
        _catalogGenerator = catalogGenerator;
        _orderingGenerator = orderingGenerator;
        _logger = logger;
    }

    public async Task RunAsync(Guid jobRunId, DummyDataTable table, long? targetRowCount, long? targetStorageBytes)
    {
        var jobRun = await _db.JobRuns.FirstOrDefaultAsync(j => j.Id == jobRunId);
        if (jobRun is null) return;

        try
        {
            var requestedRows = targetRowCount ?? (targetStorageBytes ?? 0) / AvgRowBytes;
            var rowCount = (int)Math.Clamp(requestedRows, 1, MaxRowsPerRun);

            _logger.LogInformation(
                "GenerateDummyDataJob starting (jobRunId={JobRunId}, table={Table}, rowCount={RowCount})",
                jobRunId, table, rowCount);

            var inserted = table switch
            {
                DummyDataTable.Category => await _catalogGenerator.GenerateCategoriesAsync(rowCount),
                DummyDataTable.Product => await _catalogGenerator.GenerateProductsAsync(rowCount),
                DummyDataTable.Order => await _orderingGenerator.GenerateOrdersAsync(rowCount),
                _ => throw new ArgumentOutOfRangeException(nameof(table), table, "Unknown dummy-data table"),
            };

            jobRun.Status = "Success";
            jobRun.ResultMessage = $"Generated {inserted} {table} row(s).";
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
