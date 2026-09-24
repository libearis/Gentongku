using Benchmark.Application.Abstractions;
using Benchmark.Application.DTOs;

namespace Benchmark.Infrastructure.Services;

public sealed class BenchmarkService : IBenchmarkService
{
    public Task<ReadBenchmarkResult> RunReadBenchmarkAsync(ReadBenchmarkRequest request, CancellationToken ct = default)
    {
        var results = new List<StrategyResult>
        {
            new("RedisOnly", false, 0, 0, "Not implemented yet"),
            new("DbIndexOnly", false, 0, 0, "Not implemented yet"),
            new("LiveQuery", false, 0, 0, "Not implemented yet"),
            new("IndexPlusRedis", false, 0, 0, "Not implemented yet"),
        };

        return Task.FromResult(new ReadBenchmarkResult(request.Table, request.Column, results));
    }

    public Task<WriteBenchmarkResult> RunWriteBenchmarkAsync(WriteBenchmarkRequest request, CancellationToken ct = default)
    {
        // Stubbed — not yet inserting into orders_indexed/orders_plain for a real comparison.
        return Task.FromResult(new WriteBenchmarkResult(request.RowCount, 0, 0, 0, 0));
    }
}
