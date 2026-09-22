using Benchmark.Application.DTOs;

namespace Benchmark.Application.Abstractions;

/// <summary>
/// Public contract for the Benchmark module (AGENTS.md section 6 — the
/// centerpiece module). The four-strategy read engine and write-benchmark
/// runner are intentionally TODO-stubbed in this scaffolding pass; see
/// Benchmark.Infrastructure/Services/BenchmarkService.cs for the stub and
/// AGENTS.md section 6.1/6.2 for the exact behavior to implement next.
/// </summary>
public interface IBenchmarkService
{
    Task<ReadBenchmarkResult> RunReadBenchmarkAsync(ReadBenchmarkRequest request, CancellationToken ct = default);
    Task<WriteBenchmarkResult> RunWriteBenchmarkAsync(WriteBenchmarkRequest request, CancellationToken ct = default);
}
