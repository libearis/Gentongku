using Benchmark.Application.Abstractions;
using Benchmark.Application.DTOs;

namespace Benchmark.Infrastructure.Services;

/// <summary>
/// TODO (next implementation pass, see AGENTS.md section 6.1 and 6.2):
///   - RunReadBenchmarkAsync: implement the 4 real strategies (Redis-only,
///     DB-index-only, forced live query, index+Redis) against Order/Produk/
///     Kategori, including the "no index -> degrade" behavior.
///   - RunWriteBenchmarkAsync: insert the same batch into orders_indexed and
///     orders_plain (BenchmarkDbContext) and report real rows/sec + avg ms.
/// This scaffolding pass only proves the module is wired end-to-end (DbContext
/// migrated, service registered, reachable via a trivial endpoint).
/// </summary>
public sealed class BenchmarkService : IBenchmarkService
{
    public Task<ReadBenchmarkResult> RunReadBenchmarkAsync(ReadBenchmarkRequest request, CancellationToken ct = default)
    {
        var results = new List<StrategyResult>
        {
            new("RedisOnly", false, 0, 0, "Not implemented yet — see AGENTS.md section 6.1"),
            new("DbIndexOnly", false, 0, 0, "Not implemented yet — see AGENTS.md section 6.1"),
            new("LiveQuery", false, 0, 0, "Not implemented yet — see AGENTS.md section 6.1"),
            new("IndexPlusRedis", false, 0, 0, "Not implemented yet — see AGENTS.md section 6.1"),
        };

        return Task.FromResult(new ReadBenchmarkResult(request.Table, request.Column, results));
    }

    public Task<WriteBenchmarkResult> RunWriteBenchmarkAsync(WriteBenchmarkRequest request, CancellationToken ct = default)
    {
        // Stubbed — see AGENTS.md section 6.2 for the real insert-batch comparison to implement.
        return Task.FromResult(new WriteBenchmarkResult(request.RowCount, 0, 0, 0, 0));
    }
}
