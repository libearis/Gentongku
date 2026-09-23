using Benchmark.Application.DTOs;

namespace Benchmark.Application.Abstractions;

public interface IBenchmarkService
{
    Task<ReadBenchmarkResult> RunReadBenchmarkAsync(ReadBenchmarkRequest request, CancellationToken ct = default);
    Task<WriteBenchmarkResult> RunWriteBenchmarkAsync(WriteBenchmarkRequest request, CancellationToken ct = default);
}
