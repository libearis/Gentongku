namespace Benchmark.Application.DTOs;

[Flags]
public enum ReadStrategy
{
    None = 0,
    RedisOnly = 1,
    DbIndexOnly = 2,
    LiveQuery = 4,
    IndexPlusRedis = 8
}

public sealed record ReadBenchmarkRequest(string Table, string Column, ReadStrategy EnabledStrategies);

public sealed record StrategyResult(string Strategy, bool Ran, long ElapsedMs, int RowCount, string? Note);

public sealed record ReadBenchmarkResult(string Table, string Column, IReadOnlyList<StrategyResult> Results);

public sealed record WriteBenchmarkRequest(int RowCount);

public sealed record WriteBenchmarkResult(int RowCount, double IndexedRowsPerSec, double IndexedAvgMs, double PlainRowsPerSec, double PlainAvgMs);
